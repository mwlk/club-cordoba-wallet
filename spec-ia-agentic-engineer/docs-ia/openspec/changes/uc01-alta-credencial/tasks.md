# Tasks: uc01-alta-credencial

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes cercanos (`CreateCredentialCommandHandler`, `MemberRepository`, `CredentialRepository`, `credential-create.component.ts`).
- [x] 0.3 Identificar archivos afectados (ver `proposal.md` tabla de impacto).

## 1. Validacion de spec

- [x] 1.1 Confirmar que cada escenario en `specs/alta-credencial/spec.md` sea testeable.
- [x] 1.2 Agregar escenarios faltantes antes de programar (cubiertos: alta socio nuevo/existente, falla de firma en ambos casos, generación automática, canonicalización, commit único).

## 2. Implementacion

- [ ] 2.1 Crear `IUnitOfWork` en `Application/Interfaces/IUnitOfWork.cs` (`Task SaveChangesAsync(CancellationToken ct)`).
- [ ] 2.2 Implementar `UnitOfWork` en `Infrastructure/Persistence/UnitOfWork.cs` sobre `AppDbContext.SaveChangesAsync`.
- [ ] 2.3 Registrar `IUnitOfWork` en `Infrastructure/DependencyInjection.cs`.
- [ ] 2.4 Quitar `SaveChangesAsync` de `MemberRepository.AddAsync` (dejar solo `db.Members.Add(member)`).
- [ ] 2.5 Quitar `SaveChangesAsync` de `CredentialRepository.AddAsync` (dejar solo `db.Credentials.Add(credential)`).
- [ ] 2.6 Inyectar `IUnitOfWork` en `CreateCredentialCommandHandler` y llamar `SaveChangesAsync` una sola vez, después de que `issuerService.Issue()` devuelve éxito.
- [ ] 2.7 En `credential-create.component.ts#submit()`, agregar callback `error` al `subscribe` que setee `this.submitting = false`.
- [ ] 2.8 Mantener el estilo y arquitectura del proyecto (Clean layered, CQRS manual, Result pattern, NgModules) — evitar refactors no relacionados.

## 3. Validacion

- [ ] 3.1 Agregar `CreateCredentialCommandHandlerTests` (unit, con mocks) cubriendo los 4 escenarios ADDED + los 2 MODIFIED de `spec.md`.
- [ ] 3.2 Extender `CredentialsEndpointTests` (integration, Testcontainers) con el caso "DNI nuevo + firma falla -> no hay fila en `members`".
- [ ] 3.3 Ejecutar `dotnet test tests/ClubCordobaWallet.Tests.Unit` y `dotnet test tests/ClubCordobaWallet.Tests.Integration`.
- [ ] 3.4 Verificación manual en Angular: forzar 400 de firma y confirmar que el botón de submit se reactiva.
- [ ] 3.5 Registrar evidencia en `reports/` (salida de los tests).
- [ ] 3.6 Ejecutar `/sdd-review` y resolver gaps que cambien el contrato final.

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
