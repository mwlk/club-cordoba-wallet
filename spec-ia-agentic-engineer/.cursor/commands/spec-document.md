# Comando: spec-document

Usar para cerrar la documentacion de una feature ya implementada o verificada.

Este comando NO implementa codigo. Solo completa o mejora `documentation.md` para que quede listo para Confluence o Notion.

## Prompt

```text
Estas trabajando en este repositorio.

Segui exactamente este comando: spec-document.

Cambio activo:
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/

Lee:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
5. spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md
6. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/proposal.md
7. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/design.md
8. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/**/*.md
9. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md
10. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/reports/**/* si existe
11. spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md

Objetivo:
Completar o mejorar `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md` con las 6 secciones obligatorias:

1. Que problema resuelve
2. Como deberia funcionar
3. Que se modifico, incluyendo scripts o migraciones si aplica
4. Como probarla, con evidencia
5. Que impacto tiene, riesgos y rollback
6. Como mantenerla en el futuro

Reglas:
- No modificar codigo productivo.
- No inventar evidencia: si algo no fue probado, marcarlo como pendiente.
- Usar Markdown puro compatible con Confluence y Notion.
- Usar el agente del proyecto para respetar lenguaje tecnico, arquitectura, riesgos y checklist.
- Incluir rutas reales del repo cuando existan.
- Incluir SQL, comandos, logs o pasos manuales solo si estan documentados o se pueden inferir con seguridad desde reports/tasks/design.
- Terminar indicando si la documentacion queda LISTA PARA CONFLUENCE o LISTA CON PENDIENTES.
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-document.md
CHANGE_NAME: <change-name>
```





