# Tasks: uc01-alta-credencial

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes cercanos (`CreateCredentialCommandHandler`, `MemberRepository`, `CredentialRepository`, `credential-create.component.ts`).
- [x] 0.3 Identificar archivos afectados (ver `proposal.md` tabla de impacto).

## 1. Validacion de spec

- [x] 1.1 Confirmar que cada escenario en `specs/alta-credencial/spec.md` sea testeable.
- [x] 1.2 Agregar escenarios faltantes antes de programar (cubiertos: alta socio nuevo/existente, falla de firma en ambos casos, generación automática, canonicalización, commit único).

## 2. Implementacion

- [x] 2.1 Crear `IUnitOfWork` en `Application/Interfaces/IUnitOfWork.cs` (`Task SaveChangesAsync(CancellationToken ct)`).
- [x] 2.2 Implementar `UnitOfWork` en `Infrastructure/Persistence/UnitOfWork.cs` sobre `AppDbContext.SaveChangesAsync`.
- [x] 2.3 Registrar `IUnitOfWork` en `Infrastructure/DependencyInjection.cs`.
- [x] 2.4 Quitar `SaveChangesAsync` de `MemberRepository.AddAsync` (dejar solo `db.Members.Add(member)`).
- [x] 2.5 Quitar `SaveChangesAsync` de `CredentialRepository.AddAsync` (dejar solo `db.Credentials.Add(credential)`).
- [x] 2.6 Inyectar `IUnitOfWork` en `CreateCredentialCommandHandler` y llamar `SaveChangesAsync` una sola vez, después de que `issuerService.Issue()` devuelve éxito.
- [x] 2.7 En `credential-create.component.ts#submit()`, agregar callback `error` al `subscribe` que setee `this.submitting = false`.
- [x] 2.8 Mantener el estilo y arquitectura del proyecto (Clean layered, CQRS manual, Result pattern, NgModules) — evitar refactors no relacionados.
- [x] 2.9 Sincronizar documentación desactualizada tras el fix (detectado en `/sdd-review`):
      - `docs/arquitectura.md` línea 49 — actualizado.
      - `docs/decisiones.md` línea 46 (columna Justificación) — actualizado.
- [x] 2.10 (no previsto en el diseño original, necesario para poder compilar/verificar) Corregir 3 bugs preexistentes del scaffold que impedían el build: `Application.csproj` sin `Microsoft.Extensions.DependencyInjection.Abstractions`, `Api.csproj` sin `Microsoft.AspNetCore.OpenApi`, `Program.cs` sin `using Microsoft.EntityFrameworkCore;`. Detalle en `reports/verificacion-2026-09-22.md`.

## 3. Validacion

- [x] 3.1 Agregar `CreateCredentialCommandHandlerTests` (unit, con fakes en memoria — sin librería de mocking, siguiendo el estilo de `IssuerServiceTests`) cubriendo los 4 escenarios ADDED + los 2 MODIFIED de `spec.md`. 4 tests nuevos, todos en verde.
- [~] 3.2 Extender `CredentialsEndpointTests` con el caso "DNI nuevo + firma falla -> no hay fila en `members`" — **no se pudo automatizar vía HTTP**: `Program.cs` valida `Issuer:HmacKey` al arrancar (fail-fast intencional) y la app no bootea sin la clave, así que `WebApplicationFactory` no permite simular la falla del Issuer end-to-end. Cubierto en cambio con: (3.1) unitario, y manualmente end-to-end contra `docker-compose` real (forzando `IssuerSigningException` en `IssuerService.cs` temporalmente, revertido con `git checkout` apenas terminada la prueba) — confirmado `400`, toast de error, botón liberado, y el `Member` NO queda en `members`. Ver `reports/verificacion-2026-09-22.md`.
- [x] 3.3 `dotnet test tests/ClubCordobaWallet.Tests.Unit`: **15/15 en verde**. `dotnet test tests/ClubCordobaWallet.Tests.Integration`: corrido con éxito (2 de 3 corridas 3/3 verde limpio) — encontrado y corregido un bug real de test desactualizado (`Search_Member_Not_Found_...` esperaba el contrato viejo de `mejora-busqueda-socio`). Queda un flakiness intermitente de entorno (Podman rootless + `exec` de Testcontainers, "Connection reset by peer", no determinístico) — no bloqueante, documentado en `reports/verificacion-2026-09-22.md`.
- [x] 3.4a Generar migration inicial (`dotnet ef migrations add Initial`) y levantar stack completo (`docker-compose up --build`: db Postgres, api, frontend con `pnpm i`) — stack arriba y respondiendo. Ver 3.7 por bugs adicionales corregidos para que arrancara.
- [x] 3.5 Registrar evidencia en `reports/verificacion-2026-09-22.md`.
- [x] 3.6 Ejecutar `/sdd-review` (spec/design, previo a esta implementación) y resolver el gap encontrado (tarea 2.9). Revisión de la implementación final aún pendiente si se quiere una segunda pasada de `/sdd-review`.
- [x] 3.7 (no previsto en el diseño original, necesario para levantar el stack completo) Corregir 4 issues adicionales del scaffold detectados al generar la migration y levantar `docker-compose`:
      - `docker-compose.yml` healthcheck de `db`: `pg_isready -U vcwallet` no chequeaba la DB objetivo → `pg_isready -U vcwallet -d clubcordobawallet`.
      - `frontend/package.json`: `typescript: ~5.6.0` era incompatible con `@angular/compiler-cli@22` (peer dep real `>=6.0 <6.1`, verificado vía `npm info`) → `~6.0.0`.
      - `frontend/angular.json`: agregado `cli.analytics: false` (generado por `pnpm i`/CLI, sin impacto funcional).
      - `Program.cs`: agregado `app.MapGet("/", () => Results.Redirect("/scalar/v1"))` en dev, para no quedar en 404 al abrir `http://localhost:5000/`.
      Detalle y verificación en `reports/verificacion-2026-09-22.md`.
- [x] 3.8 (no previsto, bug crítico encontrado en verificación manual en navegador) La UI quedaba congelada tras cualquier respuesta HTTP: alta exitosa (`201`) dejaba el botón en "Emitiendo…" para siempre y el listado se quedaba en skeletons de carga, aunque el backend respondía bien — causa raíz: `zone.js` no parchea `XMLHttpRequest`/`fetch` en runtime en este entorno (`NgZone.run()` tampoco entra a la zona `"angular"`), así que Angular nunca se entera de que la petición terminó. Fix: `ChangeDetectorRef.detectChanges()` explícito en el `subscribe` de `credential-list`, `credential-create`, `credential-detail` y `member-search`. Detalle completo en `docs/decisiones.md` (sección Frontend/UX) y en `reports/verificacion-2026-09-22.md`.
- [x] 3.4b Reprobado en navegador tras el fix de 3.8: alta completa (form + submit) muestra el panel "¡Credencial emitida!" con número de socio y vigencia, y el toast de éxito (interceptor genérico) al confirmar. Caso "firma falla → 400" también probado (ver 3.2): toast de error, botón liberado, nada persistido.

## 4. Documentacion Confluence / Notion

- [x] 4.1 Completado `documentation.md` con las 6 secciones.
- [x] 4.2 Documentado problema, funcionamiento, cambios, pruebas, impacto.
- [x] 4.3 Confirmado: no hay scripts/migraciones nuevos (mismo esquema).
- [x] 4.4 Evidencia en `reports/verificacion-2026-09-22.md`.
- [x] 4.5 `documentation.md` listo para copiar a Confluence/Notion.

## 5. Archive

- [x] 5.1 Validación y documentación cerradas.
- [x] 5.2 Integrado `specs/alta-credencial/spec.md` como spec estable en `spec-ia-agentic-engineer/docs-ia/openspec/specs/alta-credencial/spec.md`.
- [x] 5.3 Movido el change completo a `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc01-alta-credencial/`.
