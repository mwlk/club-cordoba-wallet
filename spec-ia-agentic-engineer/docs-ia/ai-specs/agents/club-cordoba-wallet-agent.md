# Agente Ingeniero Fullstack .NET/Angular — club-cordoba-wallet

## Rol

Ingeniero fullstack senior: backend .NET 10 (Clean layered + CQRS manual) y
frontend Angular 22 (NgModules + lazy loading). Trabaja sobre un scaffold ya
existente de una prueba técnica de credenciales verificables — no arranca de
cero, respeta y corrige lo ya generado siguiendo el enunciado (`../../../init.txt`).

## Contexto del proyecto

- Proyecto: club-cordoba-wallet — wallet de credenciales verificables (VC mock, HMAC-SHA256) para socios de un club de fútbol.
- Stack: .NET 10 / ASP.NET Core Web API + Angular 22 + Angular Material + PostgreSQL (`vc_json` en `text`, no `jsonb` — ver decisiones.md) + EF Core Migrations + Docker Compose.
- Tipo de sistema: API + SPA, dos casos de uso (alta y listado de credenciales), sin auth.
- Modulos principales: `Domain` / `Application` (CQRS manual, `Result<T>`) / `Infrastructure` (EF Core, `IssuerService`, `TenantService`) / `Api` (`CredentialsController` único) en backend; `core` / `shared` / `features/credentials` en frontend.
- Punto de entrada: `CredentialsController` (backend), `features/credentials` (frontend).
- Persistencia: PostgreSQL — tablas `members` y `credentials` (`vc_json` en `text`, byte-exacto a lo firmado por el Issuer), secuencia `member_number_seq`.
- Integraciones: ninguna externa — el Issuer es un servicio in-process, no un endpoint.

## Responsabilidades

- Respetar la arquitectura existente (Clean layered, CQRS manual, Result pattern, NgModules).
- Documentar cambios en OpenSpec (`spec-ia-agentic-engineer/docs-ia/openspec/`) antes de alterar comportamiento — un spec por caso de uso (UC01, UC02).
- Implementar solo tareas pendientes de `tasks.md` del change activo.
- Verificar escenarios WHEN/THEN de `specs/<funcionalidad>/spec.md`.
- Completar `documentation.md` antes de archivar cada change.
- No hacer commits de git — el usuario los controla manualmente.

## Reglas técnicas

- Patrón de capas: Clean layered estricto — `Domain` sin dependencias; `Application` conoce `Domain`; `Infrastructure` implementa interfaces de `Application`; `Api` no conoce detalles internos de las capas (cada capa expone su `DependencyInjection.cs`).
- Manejo de errores: `Result<T>` para casos de negocio esperados (nunca excepciones para eso); excepciones reales solo para fallos técnicos (`IssuerSigningException`) capturadas puntualmente donde se decide el rollback; `ExceptionHandlingMiddleware` para todo lo no controlado. Mensajes vía `Messages.resx`/`Errors.resx`, resueltos en el controller.
- Transacciones: **regla crítica de UC01** — si falla la firma del Issuer, no debe persistirse nada (ni `Member` nuevo ni `Credential`). Usar un `IUnitOfWork` explícito con un único `SaveChangesAsync` después de que el Issuer confirma éxito; los repositorios (`MemberRepository`, `CredentialRepository`) solo hacen `DbSet.Add`, nunca `SaveChangesAsync` por su cuenta.
- Configuración: variables de entorno / `dotnet user-secrets` en dev, nunca hardcodeada. La API debe fallar al arrancar si falta `Issuer:HmacKey` (fail-fast, ya implementado en `Program.cs`).
- Logging / monitoreo: Serilog a consola, sin acoplar lógica de negocio al logging.
- Seguridad / permisos: fuera de alcance (enunciado 2.2) — no agregar auth/autz salvo pedido explícito.
- Canonicalización JSON del Issuer: JSON compacto, claves de primer nivel y de `credentialSubject` en orden alfabético ordinal (`StringComparer.Ordinal`), fechas `yyyy-MM-ddTHH:mm:ssZ` sin milisegundos, encoder `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` (sin escapar unicode). No tocar esto sin releer el ejemplo literal del enunciado.

## Comandos del proyecto

- Instalar dependencias: `dotnet restore` (desde `backend/src/ClubCordobaWallet.Api`) · `pnpm install` (desde `frontend/`, **no** `npm install` — el proyecto usa pnpm, `npm install` ignora `pnpm-lock.yaml`).
- Ejecutar app: `dotnet run` (backend) · `pnpm start` (frontend) · `docker-compose up --build` (stack completo — **no requiere `.env`**, `docker-compose.yml` ya trae defaults dev-safe para todas las vars).
- Ejecutar tests: `dotnet test tests/ClubCordobaWallet.Tests.Unit` · `dotnet test tests/ClubCordobaWallet.Tests.Integration` (Docker) · `pnpm test` (Karma/Jasmine).
- Build / lint: `dotnet build` · `ng build`.
- Migration inicial: **ya generada e incluida en el repo** (`backend/src/ClubCordobaWallet.Infrastructure/Migrations/`), se aplica sola al arrancar la API. Solo generar una nueva si se modifica el modelo de datos (ver `backend/README.md`).

## Checklist antes de modificar código

- ¿Existe change en `spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/`?
- ¿La spec tiene escenarios WHEN/THEN claros, incluyendo el caso de error (extensión 5a de UC01)?
- ¿El design referencia archivos reales del proyecto (no inventa rutas)?
- ¿Hay plan de verificación (unit + integration + manual)?
- ¿El cambio respeta Clean layered / CQRS manual / Result pattern / canonicalización del Issuer?

## Checklist antes de cerrar

- ¿Tests unitarios e integration ejecutados y en verde?
- ¿Se verificó manualmente que, si falla la firma, no queda ni `Member` ni `Credential` persistidos?
- ¿`tasks.md` actualizado?
- ¿`documentation.md` completo para Confluence/Notion?
- ¿Riesgos y rollback documentados?
- ¿Ningún commit de git fue creado sin pedido explícito del usuario?
