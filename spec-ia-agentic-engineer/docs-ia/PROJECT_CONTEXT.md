# Contexto del proyecto: club-cordoba-wallet

> Archivo completado por `/sdd-setup` (2026-09-22).

## Qué es

Prueba técnica: sistema que emite y gestiona credenciales digitales
verificables (VC, mock) para los socios del Club de Fútbol de Córdoba,
firmadas criptográficamente (HMAC-SHA256) para garantizar su integridad.
Separa el rol de negocio (**Tenant**, el club) del rol de emisión
(**Issuer**), servicio in-process sin endpoint propio. Enunciado completo en
`../init.txt` (raíz del repo, fuera de `club-cordoba-wallet/`).

Cubre dos casos de uso: UC01 (alta de credencial de socio) y UC02 (listado de
credenciales emitidas). Explícitamente fuera de alcance: verificación de
credenciales, revocación/cambio de estado, autenticación/autorización,
fidelidad estricta a la spec W3C VC/DID, paginación.

## Stack detectado

- **Backend**: .NET 10 (`net10.0` en los 6 `.csproj`), ASP.NET Core Web API,
  Clean layered (`Domain` / `Application` / `Infrastructure` / `Api`), CQRS
  manual (interfaces propias `ICommandHandler<T,R>` / `IQueryHandler<T,R>`,
  sin MediatR), `Result<T>` pattern en vez de excepciones para casos
  esperados, mensajes vía `.resx` (`Messages.resx` / `Errors.resx`), Serilog
  (consola), middleware global `ExceptionHandlingMiddleware`.
- **Frontend**: Angular 22 (`^22.0.0`) + Angular Material, NgModules con lazy
  loading (no standalone components — decisión explícita), `ApiService` base
  + `CredentialsService`, interceptors de error y loading.
- **Base de datos**: PostgreSQL (JSONB para persistir la VC firmada tal cual,
  EF Core Migrations aplicadas automáticamente al arrancar la API). Ver
  decisión de versión en "Decisiones activas" abajo.
- **Infra**: Docker Compose (`db`, `api`, `ui`); alternativa 100% local
  documentada en `backend/README.md` / `frontend/README.md`.
- **Testing**: xUnit + FluentAssertions. Unit tests sin Docker
  (`tests/ClubCordobaWallet.Tests.Unit`); integration tests con
  Testcontainers.PostgreSql (`tests/ClubCordobaWallet.Tests.Integration`).

## Estructura

```
club-cordoba-wallet/
├── docker-compose.yml, .env.example
├── docs/
│   ├── decisiones.md      # decisiones ante puntos no especificados del enunciado
│   ├── arquitectura.md    # arquitectura y flujos UC01/UC02
│   └── modelo-datos.md    # DER y justificación del modelo
├── backend/
│   ├── src/ClubCordobaWallet.Domain/          # Member, Credential, enums, excepciones
│   ├── src/ClubCordobaWallet.Application/     # Commands/Queries, handlers, DTOs, Result
│   ├── src/ClubCordobaWallet.Infrastructure/  # EF Core, repos, IssuerService, TenantService, .resx
│   └── src/ClubCordobaWallet.Api/             # CredentialsController (único), Program.cs, middleware
├── frontend/src/app/
│   ├── core/                # servicios HTTP, interceptors, guards, modelos/DTOs
│   ├── shared/               # SharedModule: credential-card, empty-state, etc.
│   └── features/credentials/ # listado, alta, detalle (lazy loaded)
├── infra/
└── spec-ia-agentic-engineer/  # esta carpeta
```

## Puntos de entrada

- Backend: `backend/src/ClubCordobaWallet.Api/Program.cs` → `dotnet run` (puerto 5000 en Docker, ver `appsettings.Development.json` para local).
- Frontend: `npm start` en `frontend/` → `http://localhost:4200`.
- Único controller HTTP: `CredentialsController` (`GET/POST /api/credentials`, `GET /api/credentials/{id}`, `GET /api/credentials/members/search?dni=`).

## Persistencia

- `members` (uuid id, did único, member_number único secuencial zero-padded 6 dígitos, first_name, last_name, dni único, created_at).
- `credentials` (uuid id, member_id FK, `vc_json` JSONB con la VC completa firmada, created_at).
- Secuencia PostgreSQL `member_number_seq` para `numeroSocio` (persistente entre reinicios, gaps aceptados si el alta de socio se confirma pero la firma falla después).
- Migrations EF Core, **no incluidas en el scaffold** — hay que generarlas (`dotnet ef migrations add Initial ...`, ver `backend/README.md`) antes del primer arranque.

## Integraciones externas

Ninguna real. El "Issuer" es un servicio in-process (no HTTP) dentro del mismo backend — no hay integración externa que mockear ni llamar.

## Convenciones de errores

- `Result<T>` (`Success`/`Data`/`MessageKey`/`ErrorKey`) para casos de negocio esperados (socio no encontrado, credencial no encontrada, falla de firma) — el controller resuelve el mensaje final vía `ResourceManager` sobre `Messages.resx`/`Errors.resx`.
- Excepciones no controladas → `ExceptionHandlingMiddleware` (política única, controllers sin try/catch).
- Regla de negocio crítica (UC01, extensión 5a): si falla la firma en el Issuer, **no debe persistirse nada** — ni el `Member` recién creado ni la `Credential`. **Gap detectado y pendiente de fix** (ver change `uc01-alta-credencial`): hoy `MemberRepository.AddAsync` llama `SaveChangesAsync` antes de invocar al Issuer, así que un `Member` nuevo queda persistido aunque la firma falle después. La corrección decidida es introducir un `IUnitOfWork` explícito (un único `SaveChangesAsync` después de que el Issuer confirma éxito) en vez de que cada repositorio commitee por su cuenta.
- Frontend: `error.interceptor.ts` muestra snackbar global en errores HTTP. Gap detectado: `credential-create.component.ts#submit()` no tiene callback de error en el `subscribe`, así que `submitting` queda en `true` indefinidamente tras un error (aunque el snackbar sí se vea) — pendiente de fix en el mismo change.

## Convenciones de tests

- Unit tests de servicios/handlers (el más crítico: `IssuerServiceTests`, valida encoding/orden de claves/formato de fechas de la canonicalización).
- Integration tests de endpoints contra PostgreSQL real vía Testcontainers (evita falsos positivos de un motor in-memory que no reproduce JSONB/secuencias).
- "Hecho" = tests pasando + verificación manual de UC01/UC02 end-to-end (front + back + DB).

## Comandos útiles

- Backend: `dotnet restore` / `dotnet run` (desde `backend/src/ClubCordobaWallet.Api`) / `dotnet test tests/ClubCordobaWallet.Tests.Unit` / `dotnet test tests/ClubCordobaWallet.Tests.Integration` (requiere Docker).
- Frontend: `npm install` / `npm start` / `npm test` (Karma/Jasmine).
- Todo el stack: `docker-compose up --build` desde la raíz (requiere `.env` con `HMAC_SECRET_KEY`).
- Migration inicial (obligatoria antes del primer `docker-compose up` o `dotnet run`): ver `backend/README.md#primer-paso-obligatorio`.

## Reglas del proyecto

- No romper la canonicalización JSON del Issuer (orden alfabético ordinal de claves, sin escape de unicode, fechas `yyyy-MM-ddTHH:mm:ssZ`, sin milisegundos) — cualquier cambio ahí invalida la reproducibilidad de la firma HMAC. Ver ejemplo literal en `../init.txt` sección 4.1.2.
- Mantener un único controller (`CredentialsController`) — requisito explícito del enunciado (4.2.1).
- No implementar verificación de credenciales, revocación, auth/autz, ni paginación — están explícitamente fuera de alcance (enunciado, sección 2.2).
- Toda configuración sensible (clave HMAC, connection string) vía variables de entorno / user-secrets, nunca hardcodeada — la app falla al arrancar si falta `Issuer:HmacKey` (fail-fast intencional).
- Cada caso de uso (UC01, UC02) se trabaja como su propio change/spec dentro de `spec-ia-agentic-engineer/docs-ia/openspec/changes/`.
- Sin commits automáticos: el historial de commits del entregable (enunciado, sección 3) lo maneja el usuario manualmente.

## Decisiones activas (a validar/ejecutar vía SDD, no asumidas de antemano)

- **Postgres 18** (subir desde `postgres:17-alpine` en `docker-compose.yml`, aunque el enunciado no exige "última versión" para el motor de datos) — decisión confirmada por el usuario, pendiente de aplicar en `docker-compose.yml` y en las menciones a "PostgreSQL 17" de `README.md`, `docs/arquitectura.md`, `docs/modelo-datos.md`, `backend/README.md`.
- **Unit of Work** para UC01 — ver "Convenciones de errores" arriba.
