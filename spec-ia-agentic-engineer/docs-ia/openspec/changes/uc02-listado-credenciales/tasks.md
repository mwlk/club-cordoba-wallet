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

- [ ] 3.1 Confirmar que `dotnet test tests/ClubCordobaWallet.Tests.Unit` y `dotnet test tests/ClubCordobaWallet.Tests.Integration` siguen en verde (sin cambios esperados, corren igual que antes).
- [ ] 3.2 Confirmar puntualmente que `CredentialsEndpointTests.Get_Credentials_When_Empty_Returns_Empty_List` pasa (ya existe, solo se re-ejecuta como evidencia).
- [ ] 3.3 Verificación manual: apagar el backend, abrir `/credentials` y `/credentials/{id-inexistente}` y confirmar que ninguna pantalla queda en blanco indefinidamente.
- [ ] 3.4 Registrar evidencia en `reports/`.
- [ ] 3.5 Ejecutar `/sdd-review` y resolver gaps que cambien el contrato final.

## 4. Documentacion Confluence / Notion

- [ ] 4.1 Completar `documentation.md` con las 6 secciones de `spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md`.
- [ ] 4.2 Documentar problema, funcionamiento, cambios, pruebas, impacto, riesgos, rollback y mantenimiento.
- [ ] 4.3 Confirmar explícitamente: sin cambios de esquema ni scripts de BD.
- [ ] 4.4 Confirmar evidencia en `reports/` o marcar pendientes de prueba.
- [ ] 4.5 Validar que `documentation.md` está listo para copiar a Confluence/Notion.

## 5. Archive

- [ ] 5.1 Confirmar que validación y documentación están cerradas antes de archivar.
- [ ] 5.2 Integrar `specs/listado-credenciales/spec.md` como spec estable en `spec-ia-agentic-engineer/docs-ia/openspec/specs/listado-credenciales/spec.md`.
- [ ] 5.3 Mover el change completo a `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc02-listado-credenciales/`.
