# Tasks: infra-postgres-18

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes cercanos (`docker-compose.yml`, `CredentialsEndpointTests.cs`).
- [x] 0.3 Identificar archivos afectados: `docker-compose.yml`, `backend/tests/ClubCordobaWallet.Tests.Integration/Api/CredentialsEndpointTests.cs`, `README.md`, `backend/README.md`, `infra/README.md`, `docs/modelo-datos.md`, `docs/arquitectura.md`, `docs/decisiones.md` (grep completo, 8 archivos).

## 1. Implementacion

- [x] 1.1 `docker-compose.yml`: `image: postgres:17-alpine` → `postgres:18-alpine`.
- [x] 1.2 `CredentialsEndpointTests.cs`: `.WithImage("postgres:17-alpine")` → `"postgres:18-alpine"`.
- [x] 1.3 Actualizadas las 6 menciones textuales "PostgreSQL 17"/"postgres:17-alpine" en `README.md`, `backend/README.md`, `infra/README.md`, `docs/modelo-datos.md`, `docs/arquitectura.md`, `docs/decisiones.md`.
- [x] 1.4 Mantenido el estilo del proyecto.
- [x] 1.5 (no previsto) `docker-compose.yml`: volumen `pgdata:/var/lib/postgresql/data` → `pgdata:/var/lib/postgresql` — la imagen 18+ cambió la convención de layout y falla al arrancar con la ruta vieja (ver `lite.md`).

## 2. Validacion

- [x] 2.1 No aplica agregar tests nuevos (cambio de versión de imagen).
- [x] 2.2 `docker compose down` + volumen `pgdata` viejo eliminado (dato de prueba descartable, sin valor real) + `docker compose up -d db` → contenedor `postgres:18-alpine` healthy. `dotnet run` local contra esa DB: migra y arranca sin error, `GET /api/credentials` responde `[]`. `dotnet test tests/ClubCordobaWallet.Tests.Unit`: 15/15 en verde. **Pendiente real**: no se corrió `docker-compose up --build` completo (los 3 servicios vía Docker, en vez de `dotnet run`/`ng serve` locales) ni `dotnet test tests/ClubCordobaWallet.Tests.Integration` con la imagen nueva — bloqueado por el mismo problema de Podman/Testcontainers ya documentado en `uc01-alta-credencial/reports/verificacion-2026-09-22.md`.
- [x] 2.3 Escenarios de `lite.md` cubiertos salvo el de integration tests (bloqueado, no por esta spec).

## 3. Cierre

- [ ] 3.1 No hace falta `documentation.md` separado — este ajuste de infraestructura se referencia desde `documentation.md` de `uc01-alta-credencial` o `uc02-listado-credenciales` si corresponde, o se documenta con un `/sdd-document` propio si el usuario lo pide.
- [ ] 3.2 Si escala (por ejemplo, si Postgres 18 requiriera cambios de esquema/Npgsql no previstos), completar `proposal.md`/`design.md`/`specs/<funcionalidad>/spec.md` y tratarlo como full desde ahí.
- [x] 3.3 Archivado con `/sdd-archive`.
