# sdd-apply - Implementar cambio OpenSpec

Uso: `/sdd-apply CHANGE_NAME="<nombre-carpeta-en-changes>"`

Si no se pasa `CHANGE_NAME`, listar carpetas en `spec-ia-agentic-engineer/docs-ia/openspec/changes/` y pedir al usuario que elija.

---

Estas trabajando en la raiz del proyecto.

## 0. Detectar modo

Si existe `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md` -> modo **lite**.
Si no existe -> modo **full** (comportamiento de siempre, `proposal.md`+`design.md`).

## 1. Leer contexto obligatorio

1. `README.md` si existe
2. `spec-ia-agentic-engineer/AGENTS.md`
3. `spec-ia-agentic-engineer/CLAUDE.md`
4. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
5. `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md`
6. `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`
7. `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`
8. `spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md`
9. Modo lite: `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md`. Modo full: `.../proposal.md` + `.../design.md` + `.../specs/**/*.md`
10. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md`
11. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md` si existe

## 2. Reglas de implementacion

- Trabajar solo sobre tareas pendientes (`- [ ]`) en `tasks.md`, en orden.
- Si la spec es ambigua, actualizar spec/design antes de programar.
- Aplicar el rol, reglas tecnicas y checklist del agente en `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md`.
- Mantener diff minimo y estilo del proyecto.
- No hacer refactors no relacionados.
- Verificar antes de marcar tareas como completas.

## 3. Cierre

- Actualizar `documentation.md` si corresponde.
- Resumir archivos modificados, pruebas ejecutadas, escenarios cubiertos y pendientes.





