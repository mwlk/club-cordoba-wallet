# Comando: spec-archive

Usar cuando un change ya fue implementado, documentado y revisado, y queres cerrarlo formalmente.

Este comando no modifica codigo productivo. Solo actualiza specs estables si corresponde y mueve el change desde `changes/` hacia `archive/`.

## Prompt

```text
Estas trabajando en este repositorio.

Segui exactamente este comando: spec-archive.

Change a archivar:
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/

Lee:
1. README.md del proyecto si existe
2. spec-ia-agentic-engineer/AGENTS.md
3. spec-ia-agentic-engineer/CLAUDE.md si existe
4. spec-ia-agentic-engineer/docs-ia/AI_USAGE.md
5. spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
6. spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md
7. spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
8. spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md
9. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/proposal.md
10. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/design.md
11. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md
12. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md
13. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/**/*.md
14. spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/reports/**/* si existe
15. spec-ia-agentic-engineer/docs-ia/openspec/specs/**/*.md si existe

Validaciones antes de archivar:
- Confirmar que existe la carpeta del change.
- Confirmar que existen proposal.md, design.md, tasks.md, documentation.md y specs/.
- Confirmar que la seccion 3 de tasks.md refleja validacion/review ejecutada.
- Confirmar que documentation.md esta completa o que deja pendientes explicitos.
- Confirmar que la seccion 4 de tasks.md refleja documentacion completa.
- Confirmar que tasks.md esta completo o que deja pendientes justificados.
- Confirmar que no hay gaps abiertos que cambien el contrato final.
- Confirmar que no existe ya spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/. Si existe, frenar y pedir confirmacion.
- Si hay dudas, frenar y pedir confirmacion antes de mover archivos.

Orden obligatorio:
1. Validar primero: la implementacion debe estar revisada contra la spec.
2. Documentar despues: documentation.md debe estar completo o con pendientes explicitos.
3. Archivar al final: recien entonces actualizar spec estable y mover la carpeta.

Contrato estable:
- Para cada spec en changes/<CHANGE_NAME>/specs/<funcionalidad>/spec.md, decidir si debe fusionarse en spec-ia-agentic-engineer/docs-ia/openspec/specs/<funcionalidad>/spec.md.
- Si la feature agrega o modifica comportamiento permanente, actualizar o crear la spec estable.
- Si fue prueba, spike o cambio descartado, no fusionar como contrato vigente y explicarlo en el cierre.
- Si hubo MODIFIED o REMOVED, dejar la spec estable reflejando el estado final real.

Movimiento:
- Crear spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/ si no existe.
- Antes de mover, actualizar tasks.md:
  - marcar como completa la tarea de confirmacion de validacion/documentacion si corresponde.
  - marcar como completa la tarea de integrar spec estable si se hizo, o dejarla justificada si no aplica.
  - marcar como completa la tarea de mover a archive solo despues de mover correctamente.
- Mover el contenido completo de spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/ hacia spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/.
- No mover la carpeta changes/<CHANGE_NAME> dentro de archive/<CHANGE_NAME>, porque eso crea archive/<CHANGE_NAME>/<CHANGE_NAME>/.
- Si se usa PowerShell, mover los hijos de la carpeta:
  `Get-ChildItem changes/<CHANGE_NAME> | Move-Item -Destination archive/<CHANGE_NAME>`
- Eliminar la carpeta vacia spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/.
- No modificar codigo productivo.

Terminar con:
1. spec estable creada o actualizada
2. carpeta archivada
3. archivos movidos
4. pendientes si quedaron
5. confirmacion de que el change ya no esta en changes/
```

## Uso

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-archive.md
CHANGE_NAME: <change-name>
```
