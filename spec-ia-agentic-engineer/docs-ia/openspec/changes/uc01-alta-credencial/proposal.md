# Proposal: uc01-alta-credencial

## Por que

El enunciado (`init.txt`, UC01 y sección 4.1.2) exige que la emisión de una
credencial sea todo-o-nada: si falla la firma en el Issuer, **no debe
persistirse nada**. El scaffold actual del backend (`MemberRepository.AddAsync`)
llama `SaveChangesAsync` al crear un `Member` nuevo **antes** de invocar al
Issuer, así que si la firma falla después, el `Member` recién creado queda
persistido en la base — viola la extensión 5a de UC01. Además, el frontend
(`credential-create.component.ts#submit()`) no maneja el callback de error del
`Observable`, así que tras un fallo de firma el formulario queda con
`submitting = true` indefinidamente (el snackbar de error sí se ve, pero el
formulario no se libera para reintentar).

Este change formaliza UC01 completo como spec (para trazabilidad) y corrige
puntualmente esos dos gaps sobre el código ya existente — no reescribe el
scaffold.

## Que cambia

- Se agrega `IUnitOfWork` (`Application/Interfaces/IUnitOfWork.cs` +
  implementación en `Infrastructure` sobre `AppDbContext`) para que la
  persistencia de `Member` + `Credential` sea un único `SaveChangesAsync`,
  ejecutado solo después de que `IssuerService.Issue()` confirma éxito.
- `MemberRepository.AddAsync` y `CredentialRepository.AddAsync` dejan de
  llamar `SaveChangesAsync` por su cuenta (pasan a `Add`/`AddAsync` sin
  commit); `CreateCredentialCommandHandler` pasa a depender de
  `IUnitOfWork.SaveChangesAsync(ct)`.
- `credential-create.component.ts#submit()` agrega callback de `error` al
  `subscribe` que resetea `submitting = false` (el snackbar global de
  `error.interceptor.ts` ya informa el mensaje; falta liberar el estado del
  formulario).

## Que no cambia

- El contrato HTTP de `POST /api/credentials` (mismo request/response).
- La lógica de canonicalización/firma del `IssuerService` (ya correcta según
  el enunciado, verificado carácter a carácter contra el ejemplo de 4.1.2).
- El modelo de datos (`members`, `credentials`, `member_number_seq`) — mismo
  esquema, solo cambia cuándo se hace `SaveChanges`.

## Funcionalidades

### Nuevas

- `alta-credencial` (UC01 completo, formalizado como spec estable)
- `IUnitOfWork` como mecanismo de persistencia transaccional

### Modificadas

- `CreateCredentialCommandHandler` (usa `IUnitOfWork` en vez de que cada
  repo commitee)
- `MemberRepository.AddAsync`, `CredentialRepository.AddAsync` (sin
  `SaveChangesAsync` propio)
- `credential-create.component.ts` (maneja error de submit)

## Impacto

| Area | Ubicacion | Impacto |
|------|-----------|---------|
| API | `backend/src/ClubCordobaWallet.Api/Controllers/CredentialsController.cs` | Ninguno (mismo contrato) |
| Aplicación | `backend/src/ClubCordobaWallet.Application/Credentials/Commands/CreateCredential/CreateCredentialCommandHandler.cs`, `Application/Interfaces/IUnitOfWork.cs` (nuevo) | Handler pasa a orquestar el commit único vía `IUnitOfWork` |
| Infraestructura | `backend/src/ClubCordobaWallet.Infrastructure/Repositories/MemberRepository.cs`, `CredentialRepository.cs`, `Persistence/UnitOfWork.cs` (nuevo), `DependencyInjection.cs` | Se registra `IUnitOfWork`; repos dejan de hacer commit |
| UI | `frontend/src/app/features/credentials/pages/credential-create/credential-create.component.ts` | Agrega manejo de error en `submit()` |
| Datos | `members`, `credentials` | Sin cambio de esquema — cambia el momento del commit |
| Integraciones | Ninguna | — |
