# Design: uc01-alta-credencial

## Contexto

Flujo actual (`CreateCredentialCommandHandler.Handle`, verificado en el
código):

1. `memberRepository.GetByDniAsync(dni)` — busca socio existente.
2. Si no existe: `memberRepository.AddAsync(member, ct)` → **hoy hace
   `db.Members.Add(member); await db.SaveChangesAsync(ct);`** — commit
   inmediato.
3. `tenantService.BuildSubject(...)`.
4. `issuerService.Issue(subject, status, ct)` — puede lanzar
   `IssuerSigningException` si falta `Issuer:HmacKey`.
5. Si falla: `return Result.Fail(...)` — pero el `Member` del paso 2 **ya
   está persistido**.
6. Si tiene éxito: `credentialRepository.AddAsync(credential, ct)` → hoy
   también hace `SaveChangesAsync` propio.

Esto rompe UC01 extensión 5a (`init.txt` línea 71: "Si falla la firma en el
servicio Issuer, no se persiste nada") cuando el DNI es nuevo.

## Decisiones

- **Unit of Work explícito** (`IUnitOfWork.SaveChangesAsync(CancellationToken)`
  sobre `AppDbContext.SaveChangesAsync`), inyectado en el handler de
  aplicación. Se prefiere esto sobre envolver todo en una
  `IDbContextTransaction` explícita porque un único `SaveChangesAsync` de EF
  Core ya es atómico por sí mismo (una sola transacción implícita) siempre
  que ambos `Add` ocurran sobre el mismo `DbContext` antes del único commit
  — no hace falta manejar transacciones manuales para este caso.
- Los repositorios (`MemberRepository`, `CredentialRepository`) dejan de
  decidir cuándo se commitea — su responsabilidad pasa a ser solo
  `DbSet.Add`/consultas. El *orquestador* de la transacción es el
  `CommandHandler`, que es quien conoce la regla de negocio (no persistir
  nada si falla la firma).
- Se mantiene `IMemberRepository.AddAsync` / `ICredentialRepository.AddAsync`
  como `Task` (sin retorno), pero se les quita el `SaveChangesAsync` interno;
  se agrega `IUnitOfWork` como nueva interfaz en `Application/Interfaces/`.
- No se toca `NextMemberNumberAsync` (usa `nextval()` directo sobre la
  secuencia de Postgres, fuera del ciclo de vida del `DbContext` — ya es
  atómico e independiente del `SaveChanges`, y los gaps ya están aceptados y
  documentados en `docs/decisiones.md`).

## Flujo propuesto

```text
POST /api/credentials { nombre, apellido, dni, categoria, foto }
  -> CredentialsController.Create
  -> CreateCredentialCommandHandler.Handle
       -> memberRepository.GetByDniAsync(dni)
       -> si no existe: member = Member.Create(...); memberRepository.Add(member)   [sin commit]
       -> subject = tenantService.BuildSubject(member, categoria, foto)
       -> issued = issuerService.Issue(subject, active, ct)
            -> si falla: return Result.Fail("IssuerSigningFailed")   [ningún Add fue commiteado -> nada persiste]
       -> credential = Credential.Create(member.Id, issued.VcJson); credentialRepository.Add(credential)   [sin commit]
       -> unitOfWork.SaveChangesAsync(ct)   [commit único: Member (si es nuevo) + Credential]
       -> return Result.Ok(CreateCredentialResult(...))
```

## Contrato técnico

- Endpoint: `POST /api/credentials` (sin cambios de contrato).
- Request: `{ nombre, apellido, dni, categoria, foto }` (sin cambios).
- Response: `{ success, message, data: { memberNumber, validFrom, validUntil } }` (sin cambios) / 400 con `errorKey: IssuerSigningFailed` si falla la firma.
- Errores: `IssuerSigningException` (Issuer) → `Result.Fail("IssuerSigningFailed")` → HTTP 400. Ningún otro error nuevo.

## Persistencia

- Entidades leídas: `Member` (por DNI).
- Entidades modificadas: `Member` (insert condicional), `Credential` (insert) — ambas en el mismo `SaveChangesAsync`.
- Transacción: implícita de EF Core (un `SaveChangesAsync` = una transacción); no se necesita `BeginTransaction` manual porque ambos `Add` viven en el mismo `DbContext` con lifetime *scoped* (uno por request).
- Concurrencia: sin cambios respecto al scaffold actual (no hay requisito de lock optimista en el enunciado).

## Pruebas

- Unitarias: `CreateCredentialCommandHandlerTests` (nuevo) — verificar que cuando `IIssuerService.Issue` lanza `IssuerSigningException`, ni `IMemberRepository.Add` ni `ICredentialRepository.Add` dispararon un `SaveChangesAsync` (mock de `IUnitOfWork.SaveChangesAsync` con `Times.Never`).
- Integración: extender `CredentialsEndpointTests` — caso "DNI nuevo + `Issuer:HmacKey` vacío" → `POST /api/credentials` responde 400 y, al consultar la tabla `members`, no aparece ningún registro con ese DNI.
- Manual: en Angular, forzar un 400 (mockeando o desconfigurando la clave HMAC) y confirmar que el botón de submit vuelve a estar habilitado tras el snackbar de error.

## Riesgos

- Si algún otro caller futuro asumiera que `MemberRepository.AddAsync` ya commitea (comportamiento actual), se rompería silenciosamente. Mitigación: es el único caller hoy (`CreateCredentialCommandHandler`); se deja documentado en el docstring/comentario de la interfaz.
- Cambiar el momento del commit puede exponer una condición de carrera preexistente (dos altas simultáneas con el mismo DNI nuevo) que ya existía antes de este cambio (no introducida por este fix) — no está en alcance del enunciado, no se resuelve acá, se documenta como deuda conocida.

## Rollback

Revertir el registro de `IUnitOfWork` en `Infrastructure/DependencyInjection.cs`
y devolver `SaveChangesAsync` a `MemberRepository.AddAsync` /
`CredentialRepository.AddAsync` — cambio acotado a 4 archivos de backend + 1
de frontend, sin migración de datos de por medio.
