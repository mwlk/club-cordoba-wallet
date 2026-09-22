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
- [~] 3.2 Extender `CredentialsEndpointTests` con el caso "DNI nuevo + firma falla -> no hay fila en `members`" — **no se pudo implementar vía HTTP**: `Program.cs` valida `Issuer:HmacKey` al arrancar (fail-fast intencional) y la app no bootea sin la clave, así que `WebApplicationFactory` no permite simular la falla del Issuer end-to-end. El escenario queda cubierto únicamente a nivel unitario (3.1). Ver justificación en `reports/verificacion-2026-09-22.md`.
- [~] 3.3 `dotnet test tests/ClubCordobaWallet.Tests.Unit`: **12/12 en verde**. `dotnet test tests/ClubCordobaWallet.Tests.Integration`: **bloqueado en este entorno** — Testcontainers no puede levantar Postgres bajo Podman rootless (falla el wait-strategy `pg_isready` vía `exec`, error de infraestructura de test, no del código). Pendiente correr en máquina con Docker Engine real.
- [ ] 3.4 Verificación manual en Angular: pendiente — no se corrió `ng serve`/navegador en esta sesión.
- [x] 3.5 Registrar evidencia en `reports/verificacion-2026-09-22.md`.
- [x] 3.6 Ejecutar `/sdd-review` (spec/design, previo a esta implementación) y resolver el gap encontrado (tarea 2.9). Revisión de la implementación final aún pendiente si se quiere una segunda pasada de `/sdd-review`.

## 4. Documentacion Confluence / Notion

- [ ] 4.1 Completar `documentation.md` con las 6 secciones de `spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md`.
- [ ] 4.2 Documentar problema, funcionamiento, cambios, scripts/migraciones, pruebas, impacto, riesgos, rollback y mantenimiento.
- [ ] 4.3 Confirmar que no hay scripts/migraciones nuevos (mismo esquema) — dejarlo explícito.
- [ ] 4.4 Confirmar evidencia en `reports/` o marcar pendientes de prueba.
- [ ] 4.5 Validar que `documentation.md` está listo para copiar a Confluence/Notion.

## 5. Archive

- [ ] 5.1 Confirmar que validación y documentación están cerradas antes de archivar.
- [ ] 5.2 Integrar `specs/alta-credencial/spec.md` como spec estable en `spec-ia-agentic-engineer/docs-ia/openspec/specs/alta-credencial/spec.md`.
- [ ] 5.3 Mover el change completo a `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc01-alta-credencial/`.
