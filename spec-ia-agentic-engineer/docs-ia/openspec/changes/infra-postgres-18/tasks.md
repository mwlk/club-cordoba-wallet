# Tasks: infra-postgres-18

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes cercanos (`docker-compose.yml`, `CredentialsEndpointTests.cs`).
- [x] 0.3 Identificar archivos afectados: `docker-compose.yml`, `backend/tests/ClubCordobaWallet.Tests.Integration/Api/CredentialsEndpointTests.cs`, `README.md`, `backend/README.md`, `infra/README.md`, `docs/modelo-datos.md`, `docs/arquitectura.md`, `docs/decisiones.md` (grep completo, 8 archivos).

## 1. Implementacion

- [ ] 1.1 `docker-compose.yml`: `image: postgres:17-alpine` → `postgres:18-alpine`.
- [ ] 1.2 `CredentialsEndpointTests.cs`: `.WithImage("postgres:17-alpine")` → `"postgres:18-alpine"`.
- [ ] 1.3 Actualizar las 6 menciones textuales "PostgreSQL 17"/"postgres:17-alpine" en `README.md`, `backend/README.md`, `infra/README.md`, `docs/modelo-datos.md`, `docs/arquitectura.md`, `docs/decisiones.md`.
- [ ] 1.4 Mantener el estilo y arquitectura del proyecto — no tocar nada más en esos archivos.

## 2. Validacion

- [ ] 2.1 No aplica agregar tests nuevos (cambio de versión de imagen).
- [ ] 2.2 Ejecutar `docker-compose up --build` completo y confirmar que `db`, `api` y `ui` levantan; correr `dotnet test tests/ClubCordobaWallet.Tests.Integration` y confirmar que Testcontainers usa `postgres:18-alpine` y los tests pasan.
- [ ] 2.3 Confirmar los escenarios de `lite.md` cubiertos.

## 3. Cierre

- [ ] 3.1 No hace falta `documentation.md` separado — este ajuste de infraestructura se referencia desde `documentation.md` de `uc01-alta-credencial` o `uc02-listado-credenciales` si corresponde, o se documenta con un `/sdd-document` propio si el usuario lo pide.
- [ ] 3.2 Si escala (por ejemplo, si Postgres 18 requiriera cambios de esquema/Npgsql no previstos), completar `proposal.md`/`design.md`/`specs/<funcionalidad>/spec.md` y tratarlo como full desde ahí.
- [ ] 3.3 Archivar con `/sdd-archive` cuando esté listo.
