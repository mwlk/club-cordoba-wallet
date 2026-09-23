# Backend — Club Córdoba Wallet API

.NET 10 · PostgreSQL 18 · Clean layered (Domain / Application / Infrastructure / Api) · CQRS manual

## Primer paso obligatorio — generar la migration inicial

Este scaffold define el `DbContext` y las configuraciones de EF Core, pero **no incluye la carpeta `Migrations/` generada** (requiere el SDK de .NET para crearla). Antes de levantar el proyecto por primera vez:

```bash
cd src/ClubCordobaWallet.Api
dotnet tool install --global dotnet-ef   # si no lo tenés instalado
dotnet ef migrations add Initial --project ../ClubCordobaWallet.Infrastructure --startup-project .
```

Esto crea `src/ClubCordobaWallet.Infrastructure/Migrations/` con el schema inicial (tablas `members`, `credentials`, secuencia `member_number_seq`). Las migrations se aplican automáticamente al arrancar la API (`Program.cs`).

## Configurar secretos (desarrollo local)

```bash
cd src/ClubCordobaWallet.Api
dotnet user-secrets init
dotnet user-secrets set "Issuer:HmacKey" "una-clave-secreta-cualquiera-para-dev"
```

Sin esta clave configurada, la API **no arranca** (fail-fast intencional — ver `Program.cs`).

## Levantar con Docker

Desde la raíz del repo:

```bash
cp .env.example .env   # completar HMAC_SECRET_KEY con un valor propio
docker-compose up --build
```

La migration se aplica sola al levantar el contenedor `api`. La API queda en `http://localhost:5000`.

## Levantar en local (sin Docker)

Requiere PostgreSQL corriendo localmente (o vía `docker-compose up db`).

```bash
cd src/ClubCordobaWallet.Api
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
