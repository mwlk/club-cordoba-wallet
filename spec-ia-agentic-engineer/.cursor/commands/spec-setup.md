# Comando: spec-setup

Usar este comando una sola vez despues de configurar esta carpeta dentro de un proyecto.

Objetivo: completar el contexto base del proyecto para que despues cada feature pueda arrancar con `spec-new` sin volver a explicar toda la arquitectura.

## Prompt

```text
Estas trabajando en la raiz del proyecto que contiene la carpeta spec-ia-agentic-engineer.

Segui exactamente este comando: spec-setup.

Lee:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/README.md
5. spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
6. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
7. spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
8. spec-ia-agentic-engineer/docs-ia/ai-specs/_templates/agent.md

Analiza la arquitectura real del proyecto:
- carpetas principales
- stack tecnico
- comandos para instalar, ejecutar, testear y buildear
- capas/modulos
- puntos de entrada
- persistencia
- integraciones externas
- convenciones de errores
- convenciones de tests
- reglas importantes para no romper el proyecto
- rol que debe asumir la IA en este proyecto

Actualiza solamente archivos de documentacion/configuracion de IA dentro de spec-ia-agentic-engineer:
- spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
- spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md
- spec-ia-agentic-engineer/AGENTS.md si necesita reglas especificas del proyecto
- spec-ia-agentic-engineer/CLAUDE.md si necesita prompt especifico del proyecto
- spec-ia-agentic-engineer/docs-ia/AI_USAGE.md si falta un paso operativo propio del proyecto
- spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md como agente principal del proyecto

El agente principal debe quedar escrito con datos reales del proyecto y contener:
- rol
- contexto del proyecto
- responsabilidades
- reglas tecnicas
- comandos del proyecto
- checklist antes de modificar codigo
- checklist antes de cerrar

No implementes features.
No modifiques codigo productivo.
No borres documentacion existente.

Termina con:
1. resumen de arquitectura detectada
2. comandos detectados
3. agente principal creado o actualizado
4. archivos actualizados
5. proximo comando para crear una feature: Run @spec-ia-agentic-engineer/.cursor/commands/spec-new.md
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-setup.md
```


