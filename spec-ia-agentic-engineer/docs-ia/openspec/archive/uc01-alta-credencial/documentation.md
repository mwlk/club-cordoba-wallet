# Documentacion: uc01-alta-credencial

> **Publicar:** Confluence o Notion (Markdown).
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.

## Como pegar en Notion

1. Abrir este archivo en el repo y copiar todo el Markdown.
2. En Notion: pagina nueva -> pegar. Notion convierte encabezados, listas y tablas.
3. Si la tabla no se ve bien: menu del bloque -> Turn into -> Table, o Import -> Markdown y elegir el archivo.
4. Los bloques ` ```sql `, ` ```json ` o ` ```text ` deben quedar como Code; si no, crear bloque Code y pegar el contenido.
5. No usar HTML (`<br>`, entidades `&lt;`); usar Markdown compatible con Confluence y Notion.

## Como pegar en Confluence

Pegar como Markdown si el editor lo soporta, o copiar seccion por seccion. Mantener la misma fuente que Notion.

| Campo | Valor |
| --- | --- |
| Feature | Alta de credencial de socio (UC01) |
| Ticket / referencia | Prueba técnica — `init.txt`, sección 4.1.1 (UC01) |
| Estado | Aplicado y verificado |
| Fecha | 2026-09-23 |
| Responsable | — |

---

## 1. Que problema resuelve

- **Situacion anterior:** el alta de credencial persiste el `Member` nuevo antes de invocar al Issuer; si la firma falla, el `Member` queda en la base aunque el enunciado exige que no se persista nada.
- **Necesidad:** cumplir la extensión 5a de UC01 ("si falla la firma, no se persiste nada") de forma real, no solo documentada.
- **Resultado esperado tras el cambio:** alta y firma atómicas — o se persisten `Member`+`Credential` juntos, o no se persiste ninguno de los dos.

---

## 2. Como deberia funcionar

- **Flujo principal:** ver `design.md` (flujo propuesto con `IUnitOfWork`).
- **Reglas de negocio:** ver escenarios de `specs/alta-credencial/spec.md`.
- **Casos limite / errores esperados:** falla de firma con socio nuevo, falla de firma con socio existente.
- **Configuracion:** sin cambios — sigue dependiendo de `Issuer:HmacKey` (env var / user-secrets).

---

## 3. Que se modifico

- `Application/Interfaces/IUnitOfWork.cs` (nuevo): `SaveChangesAsync(ct)`.
- `Infrastructure/Persistence/UnitOfWork.cs` (nuevo): implementación sobre `AppDbContext`.
- `Infrastructure/DependencyInjection.cs`: registro de `IUnitOfWork`.
- `Infrastructure/Repositories/MemberRepository.cs`, `CredentialRepository.cs`: `AddAsync` deja de llamar `SaveChangesAsync` (solo `Add`).
- `Application/Features/Credentials/Commands/CreateCredential/CreateCredentialCommandHandler.cs`: inyecta `IUnitOfWork`, un único `SaveChangesAsync` después de que el Issuer confirma éxito.
- `frontend/.../credential-create.component.ts#submit()`: agrega callback `error` al `subscribe` (libera `submitting`).
- `docs/arquitectura.md`, `docs/decisiones.md`: sincronizados con el comportamiento real.

### Base de datos / persistencia

Sin cambios de esquema ni scripts de BD. Cambia únicamente el momento del
commit (antes: por repositorio; ahora: único, vía `IUnitOfWork`, tras el
éxito del Issuer).

---

## 4. Como probarla

- Unit tests: `dotnet test tests/ClubCordobaWallet.Tests.Unit` — 15/15 en
  verde, incluyendo `CreateCredentialCommandHandlerTests` (4 escenarios:
  falla de firma con socio nuevo/existente, éxito con socio nuevo, reuso de
  socio existente) e `IssuerServiceTests` preexistentes.
- Integration tests: `dotnet test tests/ClubCordobaWallet.Tests.Integration`
  contra `postgres:18-alpine` (Testcontainers) — 3/3 en verde en corridas
  limpias; flakiness intermitente conocido del entorno (Podman rootless +
  `exec`, no relacionado al código, ver `reports/verificacion-2026-09-22.md`).
- Manual end-to-end contra `docker-compose up --build` (los 3 servicios,
  build de producción real, no dev server): alta exitosa → confirmación +
  toast + listado actualizado al toque; alta con firma forzada a fallar
  (`IssuerSigningException` temporal, revertida) → `400`, toast de error,
  botón liberado, `Member` **no** persistido. Detalle completo en
  `reports/verificacion-2026-09-22.md`.

---

## 5. Que impacto tiene

- Corrige un incumplimiento real de la extensión 5a del enunciado (antes,
  un `Member` nuevo quedaba persistido aunque la firma fallara).
- Sin cambio de contrato HTTP ni de esquema — bajo riesgo de romper otros
  consumidores (no hay otros, es el único cliente).
- De paso resolvió un bug crítico no relacionado (UI que no refrescaba tras
  ninguna respuesta HTTP, causa raíz en el entorno — ver
  `docs/decisiones.md`, sección Frontend/UX) detectado durante la
  verificación manual de este mismo change.

---

## 6. Como mantenerla en el futuro

- **Spec OpenSpec:** `spec-ia-agentic-engineer/docs-ia/openspec/specs/alta-credencial/spec.md` (tras archivar) y `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc01-alta-credencial/`.
- **Deuda / mejoras:** condición de carrera preexistente en altas simultáneas con el mismo DNI nuevo (ver `design.md`, sección Riesgos) — no resuelta en este change, fuera del alcance del enunciado.

---

## Referencias en el repo

- `proposal.md`
- `design.md`
- `specs/alta-credencial/spec.md`
- `tasks.md`
