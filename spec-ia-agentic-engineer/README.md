# Spec IA Agentic Engineer

Plantilla generica para trabajar con IA usando un flujo spec-driven.

La idea es simple: copiar esta carpeta dentro de cualquier proyecto. Todo vive dentro de `spec-ia-agentic-engineer/`. Se configura leyendo la arquitectura del proyecto padre; no desparrama archivos en la raiz.

> Nota (este repo): el instalador `Install-SpecIa.ps1` era para Windows/PowerShell y se elimino — este proyecto corre en Linux (Fedora). El contexto se completa manualmente o con `/sdd-setup`.

## Como queda en un proyecto

```text
MiProyecto/
+-- spec-ia-agentic-engineer/
|   +-- AGENTS.md
|   +-- CLAUDE.md
|   +-- codex.md
|   +-- GEMINI.md
|   +-- .cursor/
|   |   +-- commands/
|   |   +-- rules/
|   +-- .claude/
|   |   +-- commands/
|   +-- docs-ia/
|       +-- PROJECT_CONTEXT.md
|       +-- AI_USAGE.md
|       +-- openspec/
+-- src/
+-- README.md
```

## Instalacion / configuracion inicial

1. Copiar o clonar esta carpeta dentro del proyecto.
2. Completar manualmente o con `/sdd-setup` (opencode) / `spec-setup` (Cursor):

```text
spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
```

No implementa features. No toca codigo productivo. No copia archivos fuera de `spec-ia-agentic-engineer/`.

El agente especializado del proyecto se crea en el paso `spec-setup` / `sdd-setup`, porque requiere que la IA analice la arquitectura real. Ese agente queda en:

```text
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

Si falta contexto, `/sdd-setup` lo regenera/editame.


## Activar slash commands

La plantilla guarda los comandos en:

```text
spec-ia-agentic-engineer/.claude/commands/   (formato Claude)
spec-ia-agentic-engineer/.cursor/commands/   (formato Cursor)
```

- **opencode**: los comandos quedan en `.opencode/command/` en la raiz del proyecto — `/sdd-setup`, `/sdd-new`, `/sdd-new-lite`, `/sdd-apply`, `/sdd-review`, `/sdd-document`, `/sdd-archive`.
- **Claude**: copiar los `.claude/commands/*.md` a `.claude/commands/` de la raiz para usar `/sdd-*` directo.
- **Cursor**: correr los prompts `@spec-ia-agentic-engineer/.cursor/commands/spec-new.md` etc.

Uso correcto en Claude:

```text
/sdd-setup
```

No usar:

```text
/sdd-setup.md
```
## Primer paso con IA

Cursor:

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-setup.md
```

Claude Code:

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-setup.md y ejecuta ese flujo.
```

Resultado esperado:

```text
spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

Despues, cada feature lee ese agente para mantener el rol, reglas tecnicas y checklist del proyecto.

Si queres slash commands reales de Claude (`/sdd-setup`, `/sdd-new`, etc.), Claude normalmente espera `.claude/commands` en la raiz del proyecto. En ese caso podes copiar manualmente:

```text
spec-ia-agentic-engineer/.claude/commands -> .claude/commands
```

Pero la plantilla principal sigue siendo carpeta unica.

## Flujo por feature

Cursor:

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-new.md
Run @spec-ia-agentic-engineer/.cursor/commands/spec-apply.md
Run @spec-ia-agentic-engineer/.cursor/commands/spec-review.md
Run @spec-ia-agentic-engineer/.cursor/commands/spec-document.md
Run @spec-ia-agentic-engineer/.cursor/commands/spec-archive.md
```

Claude, usando los prompts dentro de la carpeta:

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-new.md y ejecutalo para CHANGE_NAME="<change-name>" REQUIREMENT="<descripcion>".
Lee spec-ia-agentic-engineer/.claude/commands/sdd-apply.md y ejecutalo para CHANGE_NAME="<change-name>".
Lee spec-ia-agentic-engineer/.claude/commands/sdd-review.md y ejecutalo para CHANGE_NAME="<change-name>".
Lee spec-ia-agentic-engineer/.claude/commands/sdd-document.md y ejecutalo para CHANGE_NAME="<change-name>".
Lee spec-ia-agentic-engineer/.claude/commands/sdd-archive.md y ejecutalo para CHANGE_NAME="<change-name>".
```

Con slash commands reales de Claude:

```text
/sdd-new CHANGE_NAME="<change-name>" REQUIREMENT="<descripcion>"
/sdd-apply CHANGE_NAME="<change-name>"
/sdd-review CHANGE_NAME="<change-name>"
/sdd-document CHANGE_NAME="<change-name>"
/sdd-archive CHANGE_NAME="<change-name>"
```

La guia completa para usuarios esta en:

```text
spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
```

## Flujo mental

```text
copio carpeta -> configuro contexto (manual o /sdd-setup) -> spec-new por feature -> spec-apply -> spec-review -> spec-document -> spec-archive
```

Archive es el cierre formal del change: se fusiona la spec estable si corresponde y se mueve el change completo desde `docs-ia/openspec/changes/<change-name>/` hacia `docs-ia/openspec/archive/<change-name>/`.

La spec estable queda como fuente de verdad para futuros cambios. La carpeta archivada conserva el historial completo: por que se hizo, que se descarto, que se aprobo y como se verifico.

Conviene archivar cuando el cambio esta en produccion o ya no tiene iteraciones pendientes. Si el review dejo gaps abiertos, resolverlos antes para que la spec estable refleje el estado final real.

La explicacion completa esta en:

```text
spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
```

## Que problema resuelve

- Reduce prompts largos repetidos.
- Deja contexto versionado dentro del proyecto.
- Obliga a la IA a trabajar con contrato antes de tocar codigo.
- Mejora trazabilidad: requisito, diseno, tareas, pruebas y documentacion final.
- Facilita pasar el resultado a Confluence o Notion.
## Como probarlo en un proyecto nuevo

Esta prueba sirve para validar que la plantilla funciona antes de usarla en un repo real.

### 1. Crear un proyecto de prueba

```powershell
$proyecto = Join-Path $env:TEMP "ProyectoPruebaSpecIa"
mkdir $proyecto
cd $proyecto
mkdir src
Set-Content README.md "# Proyecto prueba Spec IA"
```

Opcional, para simular un proyecto Node:

```powershell
Set-Content package.json '{"scripts":{"test":"echo test"}}'
```

### 2. Copiar la carpeta de la plantilla

Copiar la carpeta completa de la plantilla (dondequiera que esté ubicada):

```powershell
Copy-Item spec-ia-agentic-engineer $proyecto -Recurse
```

La estructura debe quedar asi:

```text
ProyectoPruebaSpecIa/
+-- spec-ia-agentic-engineer/
+-- src/
+-- README.md
```

### 3. Completar el contexto base

El instalador ps1 se elimino (era Windows-only). Completar a mano o usar el comando de setup:

- **opencode**: `/sdd-setup`
- **Cursor**: `Run @spec-ia-agentic-engineer/.cursor/commands/spec-setup.md`
- **Claude**: leer `spec-ia-agentic-engineer/.claude/commands/sdd-setup.md` y ejecutar ese flujo

Resultado esperado:

```text
spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

### 4. Verificar archivos generados

```powershell
Test-Path .\spec-ia-agentic-engineer\docs-ia\PROJECT_CONTEXT.md
Test-Path .\AGENTS.md
Test-Path .\spec-ia-agentic-engineer\docs-ia\openspec\changes\example-feature
```

Resultado esperado:

```text
True
False
False
```

Significa:

- `True`: existe el contexto del proyecto.
- `False`: no hay agente del proyecto (todavia).
- `False`: no hay example por defecto.

### 5. Probar setup con IA

En Cursor:

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-setup.md
```

En Claude:

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-setup.md y ejecuta ese flujo.
No implementes features. Solo completa PROJECT_CONTEXT.md y crea el agente del proyecto.
```

### 6. Probar creacion de una feature

En Cursor:

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-new.md
CHANGE_NAME: prueba-listar-items
REQUIREMENT: Crear una pantalla o endpoint de prueba que liste items de ejemplo. No implementar codigo todavia.
```

En Claude:

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-new.md.
CHANGE_NAME: prueba-listar-items
REQUIREMENT: Crear una pantalla o endpoint de prueba que liste items de ejemplo. No implementar codigo todavia.
```

Resultado esperado:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/prueba-listar-items/
+-- proposal.md
+-- design.md
+-- tasks.md
+-- documentation.md
+-- specs/<funcionalidad>/spec.md
```

### 7. Probar documentacion

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-document.md
CHANGE_NAME: prueba-listar-items
```

Resultado esperado: `documentation.md` queda con las 6 secciones listas para Confluence/Notion, o marca pendientes si faltan pruebas.

### 8. Limpiar prueba

Cuando termines:

```powershell
Remove-Item $proyecto -Recurse -Force
```


