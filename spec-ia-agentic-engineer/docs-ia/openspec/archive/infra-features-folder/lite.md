# Lite: infra-features-folder

> Versión liviana. Califica como lite: reorganización de carpetas/namespaces
> puramente mecánica (mover + renombrar), sin cambiar ningún comportamiento
> ni contrato HTTP, revert trivial (`git mv` inverso), no crítico.

## Por qué

Detectado en revisión de estructura: en `ClubCordobaWallet.Application`,
`Credentials/` (la única feature de negocio) quedaba como carpeta hermana de
`Common/` e `Interfaces/` (transversales a cualquier feature). El usuario
pidió agruparla bajo una carpeta `Features/` para que la jerarquía refleje
la distinción entre código transversal y features verticales.

## Qué cambia

- `Application/Credentials/` → `Application/Features/Credentials/` (misma
  subestructura interna: `Commands/`, `Queries/`, `Dtos/`).
- Namespace `ClubCordobaWallet.Application.Credentials.*` →
  `ClubCordobaWallet.Application.Features.Credentials.*` en los 12 archivos
  movidos + sus `using` en `CredentialsController.cs`,
  `DependencyInjection.cs` y los 2 archivos de tests
  (`CreateCredentialCommandHandlerTests.cs`,
  `SearchMemberByDniQueryHandlerTests.cs`).
- `docs/arquitectura.md`: nota sobre la distinción `Common`/`Interfaces`
  (transversal) vs `Features/*` (vertical).

## Qué no cambia

- `Common/` e `Interfaces/` — quedan donde están (son transversales, no
  features).
- Ningún contrato HTTP, DTO ni lógica de negocio — sólo namespace/ubicación
  de archivos.
- `ClubCordobaWallet.Domain`, `ClubCordobaWallet.Infrastructure`,
  `ClubCordobaWallet.Api` — sin cambios estructurales (la Api ya sólo tiene
  un controller, no aplica la misma distinción).

## Contrato técnico

No aplica (refactor interno, sin superficie pública nueva).

## Escenarios

- **WHEN** se compila el backend (`dotnet build`) **THEN** compila sin
  errores con los namespaces nuevos.
- **WHEN** se corren los tests unitarios **THEN** siguen en verde (mismos
  15 tests, sólo cambia el `using` de los namespaces).
- **WHEN** se levanta la API (`dotnet run`) **THEN** arranca igual, todos
  los endpoints responden igual que antes del reorder.

## Pruebas previstas

- `dotnet build` limpio.
- `dotnet test tests/ClubCordobaWallet.Tests.Unit` — 15/15 en verde.
- Levantar la API y confirmar que los 4 endpoints responden igual
  (verificado manualmente en esta sesión).

## Riesgos y rollback

- Riesgo: mínimo — es un rename mecánico, el compilador marca cualquier
  referencia rota inmediatamente.
- Rollback: `git mv` inverso de la carpeta + revertir el namespace en los
  mismos archivos — cambio acotado y reversible en un solo commit.
