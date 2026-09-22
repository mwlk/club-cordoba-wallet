# Spec Workflow

## Crear

1. Leer contexto del proyecto en `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`.
2. Crear `spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/`.
3. Elegir modo: **full** (`/sdd-new`, completar proposal/design/tasks/spec/documentation — multi-repo,
   breaking change, datos en produccion, critico) o **lite** (`/sdd-new-lite`, completar lite+tasks —
   acotado a 1 repo, no rompe nada, revert simple, no critico). Ante la duda, full.
4. Pedir confirmacion antes de implementar.

## Implementar

1. Leer tasks.
2. Implementar tareas pendientes.
3. Verificar escenarios.
4. Actualizar documentation.

## Revisar

1. Comparar implementacion contra spec.
2. Revisar tests.
3. Revisar documentacion final.
4. Reportar PASS, PASS WITH GAPS o FAIL.

## Archivar

1. Integrar specs estables.
2. Mover change a archive.
3. Mantener documentation.md como salida para Confluence/Notion.
