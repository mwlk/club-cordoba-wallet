# Backend — Club Córdoba Wallet API

.NET 10 · PostgreSQL 18 · Clean layered (Domain / Application / Infrastructure / Api) · CQRS manual

## Migrations

La migration inicial (`Migrations/20260923002440_Initial.cs`) ya está incluida en el repo, con el schema completo (tablas `members`, `credentials`, secuencia `member_number_seq`). Se aplica sola al arrancar la API (`db.Database.Migrate()` en `Program.cs`) — no requiere ningún paso manual antes de levantar el proyecto.

Solo hace falta generar una nueva migration si se modifica el modelo de datos:

```bash
cd src/ClubCordobaWallet.Api
dotnet tool install --global dotnet-ef   # si no lo tenés instalado
dotnet ef migrations add NombreDelCambio --project ../ClubCordobaWallet.Infrastructure --startup-project .
```

## Levantar con Docker (recomendado)

Desde la raíz del repo:

```bash
docker-compose up --build
```

`docker-compose.yml` ya trae un `HMAC_SECRET_KEY` de desarrollo por default — no hace falta crear `.env` para levantar el proyecto. Para usar una clave propia: `cp .env.example .env` y completarla ahí (ver [README de la raíz](../README.md) e [infra/README.md](../infra/README.md)).

La migration se aplica sola al levantar el contenedor `api`. La API queda en `http://localhost:5000`.

## Levantar en local (sin Docker)

Requiere PostgreSQL corriendo localmente (o vía `docker-compose up db`), y configurar el secreto del Issuer:

```bash
cd src/ClubCordobaWallet.Api
dotnet user-secrets init
dotnet user-secrets set "Issuer:HmacKey" "una-clave-secreta-cualquiera-para-dev"
```

Sin esta clave configurada, la API **no arranca** (fail-fast intencional — ver `Program.cs`). Después:

```bash
dotnet restore
dotnet run
```

Connection string por defecto en `appsettings.Development.json` — ajustar usuario/contraseña si hace falta.

## Correr los tests

```bash
# Unit tests (no requieren Docker)
dotnet test tests/ClubCordobaWallet.Tests.Unit

# Integration tests (requieren Docker corriendo — usan Testcontainers)
dotnet test tests/ClubCordobaWallet.Tests.Integration
```

## Estructura

```
src/
├── ClubCordobaWallet.Domain/          # Entidades, enums, excepciones
├── ClubCordobaWallet.Application/     # Commands/Queries, handlers, DTOs, interfaces
├── ClubCordobaWallet.Infrastructure/  # EF Core, repositorios, IssuerService, TenantService
└── ClubCordobaWallet.Api/             # Controllers, middleware, Program.cs
tests/
├── ClubCordobaWallet.Tests.Unit/
└── ClubCordobaWallet.Tests.Integration/
```

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/credentials/members/search?dni=` | Busca socio por DNI (para autocompletar el form) |
| GET | `/api/credentials` | Listado de credenciales (DTO mínimo) |
| GET | `/api/credentials/{id}` | Detalle de una credencial |
| POST | `/api/credentials` | Alta de credencial (`{ nombre, apellido, dni, categoria, foto }`) |

## Documentación interactiva de la API (solo Development)

`http://localhost:5000/scalar/v1` — UI interactiva Scalar sobre el documento
OpenAPI generado por `Microsoft.AspNetCore.OpenApi`, reemplazo de Swagger UI
(ver `docs/decisiones.md`, sección Arquitectura).

Ver [docs/arquitectura.md](../docs/arquitectura.md) y [docs/decisiones.md](../docs/decisiones.md) para el detalle de diseño.
