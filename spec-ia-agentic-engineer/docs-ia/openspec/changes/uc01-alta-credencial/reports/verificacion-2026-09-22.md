# Evidencia de verificación — uc01-alta-credencial (2026-09-22)

## Build completo del backend

```text
$ dotnet build
ok dotnet build: 7 projects, 0 errors, 24 warnings (00:00:04.77)
```

Nota: el scaffold original no compilaba (bugs preexistentes, fuera del
alcance de esta spec, corregidos para poder verificar):
- `ClubCordobaWallet.Application.csproj` no referenciaba
  `Microsoft.Extensions.DependencyInjection.Abstractions` (usado en
  `DependencyInjection.cs`).
- `ClubCordobaWallet.Api.csproj` no referenciaba `Microsoft.AspNetCore.OpenApi`
  (usado por `AddOpenApi()`/`MapOpenApi()`).
- `Program.cs` no tenía `using Microsoft.EntityFrameworkCore;` (necesario
  para `db.Database.Migrate()`).

## Unit tests

```text
$ dotnet test tests/ClubCordobaWallet.Tests.Unit
Passed!  - Failed: 0, Passed: 12, Skipped: 0, Total: 12, Duration: 62 ms
```

Incluye los 4 tests nuevos de `CreateCredentialCommandHandlerTests`
(escenarios ADDED/MODIFIED de `specs/alta-credencial/spec.md`):

- `Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_New_Member`
- `Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_Existing_Member`
- `Handle_Should_Call_SaveChanges_Once_When_Issuer_Succeeds_With_New_Member`
- `Handle_Should_Reuse_Existing_Member_Did_And_Number_On_Success`

Más los 8 tests preexistentes de `IssuerServiceTests` (canonicalización,
sin cambios).

## Integration tests — BLOQUEADO por el entorno, no verificado acá

```text
$ dotnet test tests/ClubCordobaWallet.Tests.Integration
Failed! - Failed: 3, Passed: 0, Skipped: 0, Total: 3
```

Las 3 fallas son de infraestructura de test (Testcontainers no puede
levantar el `PostgreSqlContainer` en este entorno — `docker` acá es Podman
rootless, y el `exec` que usa Testcontainers para el wait-strategy de
Postgres (`pg_isready`) se cae con `Connection reset by peer`), **no** del
código de la aplicación. `docker run hello-world` funciona; el problema es
específico de la API de exec de Docker Engine que Testcontainers necesita y
que Podman no replica 1:1.

No se pudo agregar el caso de integración planeado en `tasks.md` 3.2 ("DNI
nuevo + firma falla -> no hay fila en `members`") por un motivo adicional,
de diseño: `Program.cs` valida `Issuer:HmacKey` al arrancar y **falla el
boot completo** si falta (fail-fast intencional, documentado). Eso impide
simular la falla de firma vía `WebApplicationFactory` con la clave vacía —
la app nunca llega a levantar. El escenario "falla de firma -> no se
persiste nada" queda cubierto exclusivamente a nivel unitario (arriba), que
sí puede invocar `IssuerService` fallando sin bootear la app completa.

**Pendiente real:** correr `dotnet test tests/ClubCordobaWallet.Tests.Integration`
en una máquina con Docker Engine real (no Podman) para confirmar que
`Get_Credentials_When_Empty_Returns_Empty_List`, `Full_Flow_Create_Then_List_Then_Detail`
y `Search_Member_Not_Found_Returns_200_With_Success_False` siguen pasando
después del cambio de `IUnitOfWork` (no deberían verse afectados: mismo
comportamiento observable, cambia solo el momento interno del commit).

## Verificación manual (frontend)

No ejecutada en esta sesión (no hay navegador conectado a un `ng serve`
corriendo). Pendiente: `npm start` en `frontend/`, forzar un 400 de firma y
confirmar que el botón de submit se reactiva tras el error
(`credential-create.component.ts#submit()`, callback `error` agregado).
