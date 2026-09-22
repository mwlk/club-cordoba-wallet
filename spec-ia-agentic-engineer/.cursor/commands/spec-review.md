# Comando: spec-review

Usar para revisar una implementacion contra su change.

## Prompt

```text
Estas trabajando en este repositorio.

Segui exactamente este comando: spec-review.

Cambio activo:
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/

Lee:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
5. spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md
6. spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md
7. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/proposal.md
8. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/design.md
9. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/**/*.md
10. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md
11. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md si existe
12. Archivos de implementacion afectados

Modo review:
- No editar archivos salvo pedido explicito.
- Comparar implementacion contra cada escenario.
- Identificar gaps de tests, errores, arquitectura y documentacion.
- Verificar que la implementacion respete el agente del proyecto.
- Verificar que documentation.md exista o este pedido antes de archivar.
- Terminar con PASS, PASS WITH GAPS o FAIL.
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-review.md
CHANGE_NAME: <change-name>
```







