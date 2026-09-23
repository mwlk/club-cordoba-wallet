# Tasks: infra-features-folder

## 0. Revision de contexto

- [x] 0.1 Leer estructura actual de `ClubCordobaWallet.Application` (`Common/`, `Credentials/`, `Interfaces/`).

## 1. Implementacion

- [x] 1.1 Mover `Application/Credentials/` → `Application/Features/Credentials/` (`mv`, misma subestructura interna).
- [x] 1.2 Renombrar namespace `ClubCordobaWallet.Application.Credentials.*` → `ClubCordobaWallet.Application.Features.Credentials.*` en los 12 archivos movidos (`sed` sobre using + namespace).
- [x] 1.3 Actualizar `using` en `CredentialsController.cs` y `DependencyInjection.cs`.
- [x] 1.4 Actualizar `using` en los tests (`CreateCredentialCommandHandlerTests.cs`, `SearchMemberByDniQueryHandlerTests.cs`).
- [x] 1.5 Actualizar `docs/arquitectura.md` con la distinción `Common`/`Interfaces` (transversal) vs `Features/*` (vertical).

## 2. Validacion

- [x] 2.1 `dotnet build` — 0 errores.
- [x] 2.2 `dotnet test tests/ClubCordobaWallet.Tests.Unit` — 15/15 en verde.
- [x] 2.3 `dotnet run` — API levanta igual, sin cambios de comportamiento.

## 3. Cierre

- [x] 3.1 Archivado con `/sdd-archive`.
