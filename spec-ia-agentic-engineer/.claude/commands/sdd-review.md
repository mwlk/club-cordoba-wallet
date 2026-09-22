# sdd-review - Revisar implementacion vs spec

Uso: `/sdd-review CHANGE_NAME="<nombre-carpeta-en-changes>"`

Si no se pasa `CHANGE_NAME`, listar carpetas en `spec-ia-agentic-engineer/docs-ia/openspec/changes/` y pedir al usuario que elija.

Modo revision: no editar salvo pedido explicito.

---

Estas trabajando en la raiz del proyecto.

## 0. Detectar modo

Si existe `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md` -> modo **lite**.
Si no existe -> modo **full**.

## 1. Leer contexto

1. `README.md` si existe
2. `spec-ia-agentic-engineer/AGENTS.md`
3. `spec-ia-agentic-engineer/CLAUDE.md`
4. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
5. `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md`
6. `spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md`
7. Modo lite: `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md`. Modo full: `.../proposal.md` + `.../design.md` + `.../specs/**/*.md`
8. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md`
9. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md` si existe
10. Archivos de implementacion referenciados por design/tasks/lite

## 2. Revisar

- Cobertura de escenarios WHEN/THEN.
- Contrato publico/API/UI/BD segun corresponda.
- Arquitectura y limites entre capas.
- Reglas y checklist del agente del proyecto.
- Tests y evidencia prometida en tasks.
- `documentation.md` apto Confluence/Notion, si existe (en modo lite no es obligatorio salvo que se haya corrido `/sdd-document`).
- Pendientes o riesgos no documentados.
- En modo lite: `specs/` no es obligatorio — los escenarios viven en `lite.md`.

## 3. Veredicto

Findings por severidad:

- CRITICAL
- WARNING
- INFO

Terminar con:

- PASS
- PASS WITH GAPS
- FAIL





