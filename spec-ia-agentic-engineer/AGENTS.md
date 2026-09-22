# AGENTS

Este repositorio usa una carpeta unica de trabajo IA:

`spec-ia-agentic-engineer/`

Antes de modificar codigo, leer:

1. `README.md` del proyecto si existe.
2. `spec-ia-agentic-engineer/README.md`.
3. `spec-ia-agentic-engineer/AGENTS.md`.
4. `spec-ia-agentic-engineer/docs-ia/README.md`.
5. `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`.
6. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`.
7. `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
8. El change activo en `spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/`.

Reglas:

- Dos modos de change: **full** (`proposal.md`+`design.md`+`tasks.md`+`documentation.md`+`specs/<funcionalidad>/spec.md`, via `/sdd-new`) y **lite** (`lite.md`+`tasks.md`, via `/sdd-new-lite`). Ver criterio de cual usar en `sdd-new.md`/`sdd-new-lite.md`.
- No implementar codigo si falta `tasks.md` y, segun el modo del change activo, falta `lite.md` (modo lite) o `proposal.md`+`design.md`+`specs/<funcionalidad>/spec.md` (modo full).
- Si la feature es ambigua, actualizar la spec antes de programar.
- Mantener cambios chicos y trazables.
- Marcar tareas como completas solo despues de verificar.
- Antes de archivar, completar `documentation.md` para Confluence/Notion.
- Para archivar, usar `spec-archive` o `sdd-archive`: actualizar spec estable si corresponde y mover el change completo de `openspec/changes/` a `openspec/archive/`.
- Mantener todos los artefactos IA dentro de `spec-ia-agentic-engineer/`.
