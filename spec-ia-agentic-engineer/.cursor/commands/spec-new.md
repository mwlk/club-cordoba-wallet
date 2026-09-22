# Comando: spec-new

Usar cuando quieras crear un nuevo change spec-driven.

## Prompt

```text
Estas trabajando en la raiz del proyecto que contiene la carpeta spec-ia-agentic-engineer.

Segui exactamente este comando: spec-new.

Lee en orden:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/README.md
5. spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
6. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
7. spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md
8. spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
9. spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md
10. spec-ia-agentic-engineer/docs-ia/openspec/_templates/proposal.md
11. spec-ia-agentic-engineer/docs-ia/openspec/_templates/design.md
12. spec-ia-agentic-engineer/docs-ia/openspec/_templates/tasks.md
13. spec-ia-agentic-engineer/docs-ia/openspec/_templates/spec.md
14. spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md

Objetivo:
Crear un nuevo change para esta feature:
<REQUIREMENT o FEATURE_DESCRIPTION>

Reglas:
- Crear o actualizar solo archivos bajo spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/.
- Usar los templates de spec-ia-agentic-engineer/docs-ia/openspec/_templates como estructura base.
- Usar REQUIREMENT como descripcion principal de la feature. Si no existe, usar FEATURE_DESCRIPTION.
- Generar proposal.md, design.md, tasks.md, documentation.md y specs/<funcionalidad>/spec.md.
- Incluir escenarios de aceptacion, errores, restricciones tecnicas y tareas de verificacion.
- Usar PROJECT_CONTEXT.md como contexto fijo de arquitectura del proyecto.
- Usar el agente de spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md para ajustar rol, reglas tecnicas y checklist.
- Si PROJECT_CONTEXT.md no existe o esta incompleto, frenar y pedir ejecutar spec-setup antes de crear la feature.
- Si no existe agente en spec-ia-agentic-engineer/docs-ia/ai-specs/agents/, frenar y pedir ejecutar spec-setup antes de crear la feature.
- No implementar codigo en este comando.
- Pedir confirmacion antes de implementar.
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-new.md
CHANGE_NAME: <nombre-del-cambio>
REQUIREMENT: <descripcion completa de la feature>
```


