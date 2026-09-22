# sdd-setup - Configurar contexto IA del proyecto

Uso: `/sdd-setup`

Ejecutar una sola vez despues de configurar esta carpeta dentro de un proyecto real.

No implementar features. Solo analizar arquitectura y completar contexto base.

---

Estas trabajando en la raiz del proyecto que contiene la carpeta `spec-ia-agentic-engineer`.

## 1. Leer contexto

1. `README.md` del proyecto si existe
2. `spec-ia-agentic-engineer/AGENTS.md`
3. `spec-ia-agentic-engineer/CLAUDE.md`
4. `spec-ia-agentic-engineer/docs-ia/README.md`
5. `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`
6. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
7. `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`
8. `spec-ia-agentic-engineer/docs-ia/ai-specs/_templates/agent.md`

## 2. Analizar arquitectura real

Identificar:

- carpetas principales
- stack tecnico
- comandos para instalar, ejecutar, testear y buildear
- capas o modulos
- puntos de entrada
- persistencia
- integraciones externas
- convenciones de errores
- convenciones de tests
- reglas importantes para no romper el proyecto
- rol que debe asumir la IA en este proyecto

## 3. Actualizar solo documentacion IA dentro de spec-ia-agentic-engineer

Permitido editar:

- `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
- `spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md`
- `spec-ia-agentic-engineer/AGENTS.md` si necesita reglas especificas del proyecto
- `spec-ia-agentic-engineer/CLAUDE.md` si necesita prompts especificos
- `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md` si falta algun paso operativo propio del proyecto
- `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md` como agente principal del proyecto

El agente principal debe quedar escrito con datos reales del proyecto y contener:

- rol
- contexto del proyecto
- responsabilidades
- reglas tecnicas
- comandos del proyecto
- checklist antes de modificar codigo
- checklist antes de cerrar

No modificar codigo productivo.
No crear features.

## 4. Cierre

Terminar con:

1. resumen de arquitectura detectada
2. comandos detectados
3. agente principal creado o actualizado
4. archivos actualizados
5. proximo comando: leer `spec-ia-agentic-engineer/.claude/commands/sdd-new.md` o usar `/sdd-new` si copiaste los slash commands a la raiz


