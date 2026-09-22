# Lite: infra-postgres-18

> Version liviana de proposal+design+spec. Califica como lite: queda en 1
> repo, no rompe contratos ni esquema (mismo modelo de datos, solo cambia la
> imagen del motor), revert simple (volver a `postgres:17-alpine`), no crítico
> (JSONB, secuencias y sintaxis usadas ya son compatibles entre Postgres 17 y
> 18).

## Por que

El enunciado no exige "última versión" para el motor de datos (es a criterio
del candidato), pero el usuario decidió subir de todos modos de Postgres 17 a
Postgres 18 (última estable verificada: 18.6) por consistencia con el resto
del stack (.NET 10 / Angular 22, ambos ya en su última versión).

## Que cambia

- `docker-compose.yml`: `image: postgres:17-alpine` → `postgres:18-alpine`.
- `backend/tests/ClubCordobaWallet.Tests.Integration/Api/CredentialsEndpointTests.cs`: `.WithImage("postgres:17-alpine")` → `"postgres:18-alpine"`.
- Menciones textuales "PostgreSQL 17" → "PostgreSQL 18" en: `README.md` (diagrama Mermaid + bullet de stack), `backend/README.md` (encabezado), `infra/README.md` (tabla de servicios), `docs/modelo-datos.md` (encabezado "Motor"), `docs/arquitectura.md` (diagrama Mermaid), `docs/decisiones.md` (tabla de decisiones de arquitectura).

## Que no cambia

- El modelo de datos (`members`, `credentials`, `member_number_seq`) — JSONB y `SEQUENCE` son compatibles sin cambios entre Postgres 17 y 18.
- `Npgsql.EntityFrameworkCore.PostgreSQL` sigue en `10.0.0` (compatible con Postgres 18; no hay requisito de "última versión" para paquetes NuGet secundarios).
- Ningún contrato HTTP ni lógica de negocio.

## Contrato tecnico

- No aplica (cambio de infraestructura/configuración, no de API).

## Escenarios

- **WHEN** se corre `docker-compose up --build` **THEN** el servicio `db` levanta con `postgres:18-alpine` y la API migra y conecta sin errores.
- **WHEN** se corren los integration tests (`dotnet test tests/ClubCordobaWallet.Tests.Integration`) **THEN** Testcontainers levanta `postgres:18-alpine` y los tests existentes (incluyendo `Get_Credentials_When_Empty_Returns_Empty_List` y el flujo completo) pasan igual que con 17.

## Pruebas previstas

- Unitarias: no aplica (no hay lógica de negocio nueva).
- Manual / browser: `docker-compose up --build` completo + alta y listado de una credencial de punta a punta contra el nuevo motor.

## Riesgos y rollback

- Riesgo: ninguna sintaxis SQL/JSONB usada hoy es específica de una versión — riesgo bajo.
- Rollback: volver `postgres:17-alpine` en los 2 archivos de código/config y revertir las menciones textuales — 1-2 líneas por archivo, sin migración de datos de por medio.
