# Tasks: uc02-listado-credenciales

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes cercanos (`GetCredentialsQueryHandler`, `GetCredentialByIdQueryHandler`, `credential-list.component.ts`, `credential-detail.component.ts`, `CredentialsEndpointTests`).
- [x] 0.3 Identificar archivos afectados (ver `proposal.md` tabla de impacto — solo frontend).

## 1. Validacion de spec

- [x] 1.1 Confirmar que cada escenario en `specs/listado-credenciales/spec.md` sea testeable.
- [x] 1.2 Agregar escenarios faltantes antes de programar (cubiertos: listado con datos, estado vacío, detalle expandible, no encontrada, manejo de error de red en ambas pantallas).

## 2. Implementacion

- [x] 2.1 En `credential-list.component.ts#ngOnInit`, en vez de callback `error` separado (duplicaría `detectChanges()` en dos branches), se usa `pipe(catchError(() => of([])))` antes del `subscribe`: el stream resuelve a lista vacía y un único `next` cubre éxito y fallo. `loaded = true` siempre.
- [x] 2.2 En `credential-detail.component.ts#ngOnInit`, mismo mecanismo: `pipe(catchError(() => of({success:false,message:'',data:null} as ApiResponse<CredentialDetail>)))`. El `else` existente (`notFound = true`) ya cubre tanto `success:false` real como el fallback de error — un solo código path.
- [x] 2.3 No se tocó backend (`GetCredentialsQueryHandler`, `GetCredentialByIdQueryHandler`, `CredentialsController`) — ya cumplen el enunciado.

## 3. Validacion

- [x] 3.1 `dotnet test tests/ClubCordobaWallet.Tests.Unit` → 15/15 pass. `dotnet test tests/ClubCordobaWallet.Tests.Integration` → 3/3 pass.
- [x] 3.2 `CredentialsEndpointTests.Get_Credentials_When_Empty_Returns_Empty_List` confirmado en verde dentro del run de integración.
- [x] 3.3 Verificación manual en docker-compose: backend apagado, `/credentials` y `/credentials/{id-inexistente}` no quedan en blanco (estado vacío y "no encontrada" respectivamente). Repetido con backend arriba (404 real) sin regresión.
- [x] 3.4 Evidencia registrada en `reports/verificacion-2026-09-23.md`.
- [x] 3.5 `/sdd-review` ejecutado — PASS. Único gap: `design.md` desactualizado (describía `subscribe(next,error)`, real es `catchError` en pipe) — corregido.

## 4. Documentacion Confluence / Notion

- [x] 4.1 `documentation.md` completo, 6 secciones.
- [x] 4.2 Problema, funcionamiento, cambios, pruebas, impacto, riesgos, rollback y mantenimiento documentados.
- [x] 4.3 Confirmado: sin cambios de esquema ni scripts de BD.
- [x] 4.4 Evidencia en `reports/verificacion-2026-09-23.md`.
- [x] 4.5 `documentation.md` en Markdown plano, sin HTML, listo para copiar a Confluence/Notion.

## 5. Archive

- [x] 5.1 Validación (sección 3) y documentación (sección 4) cerradas antes de archivar.
- [x] 5.2 Spec estable integrada en `spec-ia-agentic-engineer/docs-ia/openspec/specs/listado-credenciales/spec.md`.
- [x] 5.3 Change movido a `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc02-listado-credenciales/`. Archivado con `/sdd-archive`.
