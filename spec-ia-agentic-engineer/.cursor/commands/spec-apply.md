# Comando: spec-apply

Usar cuando ya existe un change y queres que la IA lo implemente.

## Prompt

```text
Estas trabajando en este repositorio.

Segui exactamente este comando: spec-apply.

Cambio activo:
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/

Lee:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
5. spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md
6. spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
7. spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
8. spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md
9. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/proposal.md
10. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/design.md
11. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/**/*.md
12. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md
13. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md si existe

Reglas:
- Trabajar solo sobre tareas pendientes.
- Si falta claridad, actualizar spec antes de programar.
- Aplicar el rol, reglas tecnicas y checklist del agente en spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md.
- Marcar tareas como completas solo despues de verificar.
- Antes de cerrar, actualizar documentation.md para Confluence/documentacion interna.
- Terminar con archivos modificados, pruebas ejecutadas y escenarios cubiertos.
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-apply.md
CHANGE_NAME: <change-name>
```







