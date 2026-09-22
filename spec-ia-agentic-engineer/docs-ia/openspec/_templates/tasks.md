# Tasks: <change-name>

## 0. Revision de contexto

- [ ] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [ ] 0.2 Inspeccionar patrones existentes cercanos.
- [ ] 0.3 Identificar archivos afectados.

## 1. Validacion de spec

- [ ] 1.1 Confirmar que cada escenario en `specs/<funcionalidad>/spec.md` sea testeable.
- [ ] 1.2 Agregar escenarios faltantes antes de programar.

## 2. Implementacion

- [ ] 2.1 Implementar el cambio minimo que satisfaga la spec.
- [ ] 2.2 Mantener el estilo y arquitectura del proyecto.
- [ ] 2.3 Evitar refactors no relacionados.

## 3. Validacion

- [ ] 3.1 Agregar o actualizar tests.
- [ ] 3.2 Ejecutar pruebas enfocadas.
- [ ] 3.3 Registrar evidencia en `reports/` si aplica.
- [ ] 3.4 Ejecutar `spec-review` o `/sdd-review` y resolver gaps que cambien el contrato final.

## 4. Documentacion Confluence / Notion

- [ ] 4.1 Completar `documentation.md` con las 6 secciones de `spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md`.
- [ ] 4.2 Documentar problema, funcionamiento, cambios, scripts/migraciones, pruebas, impacto, riesgos, rollback y mantenimiento.
- [ ] 4.3 Confirmar scripts, migraciones o configuracion requerida, o indicar explicitamente que no aplica.
- [ ] 4.4 Confirmar evidencia en `reports/` o marcar pendientes de prueba.
- [ ] 4.5 Validar que `documentation.md` esta listo para copiar a Confluence/Notion.

## 5. Archive

- [ ] 5.1 Confirmar que validacion y documentacion estan cerradas antes de archivar.
- [ ] 5.2 Integrar specs estables en `spec-ia-agentic-engineer/docs-ia/openspec/specs/` si corresponde.
- [ ] 5.3 Mover el change completo a `spec-ia-agentic-engineer/docs-ia/openspec/archive/<change-name>/`.

