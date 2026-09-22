# sdd-archive - Cerrar y archivar change OpenSpec

Uso: `/sdd-archive CHANGE_NAME="<nombre-carpeta-en-changes>"`

Si no se pasa `CHANGE_NAME`, listar carpetas en `spec-ia-agentic-engineer/docs-ia/openspec/changes/` y pedir al usuario que elija.

Este comando no modifica codigo productivo. Solo actualiza specs estables si corresponde y mueve el change desde `changes/` hacia `archive/`.

---

Estas trabajando en la raiz del proyecto.

## 0. Detectar modo

Si existe `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md` -> modo **lite**.
Si no existe -> modo **full**.

## 1. Leer contexto

1. `README.md` si existe
2. `spec-ia-agentic-engineer/AGENTS.md`
3. `spec-ia-agentic-engineer/CLAUDE.md`
4. `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`
5. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
6. `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md`
7. `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`
8. `spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md`
9. Modo lite: `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/lite.md`. Modo full: `.../proposal.md` + `.../design.md`
10. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/tasks.md`
11. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/documentation.md` si existe
12. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/**/*.md` si existe (modo full)
13. `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/reports/**/*` si existe
14. `spec-ia-agentic-engineer/docs-ia/openspec/specs/**/*.md` si existe

## 2. Validar antes de archivar

- Confirmar que existe la carpeta del change.
- Modo full: confirmar que existen `proposal.md`, `design.md`, `tasks.md`, `documentation.md` y `specs/`.
- Modo lite: confirmar que existen `lite.md` y `tasks.md` (`documentation.md` sigue opcional en ambos modos).
- Confirmar que la seccion de validacion de `tasks.md` refleja validacion/review ejecutada.
- Si existe `documentation.md`, confirmar que esta completa o que deja pendientes explicitos.
- Confirmar que `tasks.md` esta completo o que deja pendientes justificados.
- Confirmar que no hay gaps abiertos que cambien el contrato final.
- Confirmar que no existe ya `spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/`. Si existe, frenar y pedir confirmacion.
- Si hay dudas, frenar y pedir confirmacion antes de mover archivos.

Orden obligatorio:

1. Validar primero: la implementacion debe estar revisada contra la spec.
2. Documentar despues: `documentation.md` debe estar completo o con pendientes explicitos.
3. Archivar al final: recien entonces actualizar spec estable y mover la carpeta.

## 3. Actualizar contrato estable

En modo lite este paso es opcional por defecto (no hay `specs/` a menos que se haya escalado el change a full en el camino) — saltar si no aplica y decirlo explicitamente en el cierre.

Para cada spec en:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/<funcionalidad>/spec.md
```

decidir si debe fusionarse en:

```text
spec-ia-agentic-engineer/docs-ia/openspec/specs/<funcionalidad>/spec.md
```

Reglas:

- Si la feature agrega o modifica comportamiento permanente, actualizar o crear la spec estable.
- Si fue prueba, spike o cambio descartado, no fusionar como contrato vigente y explicarlo en el cierre.
- Si hubo `MODIFIED` o `REMOVED`, dejar la spec estable reflejando el estado final real.

## 4. Mover a archive

Antes de mover, actualizar `tasks.md`:

- marcar como completa la tarea de confirmacion de validacion/documentacion si corresponde.
- marcar como completa la tarea de integrar spec estable si se hizo, o dejarla justificada si no aplica.
- marcar como completa la tarea de mover a archive solo despues de mover correctamente.
- no mover la carpeta `changes/<CHANGE_NAME>` dentro de `archive/<CHANGE_NAME>`, porque eso crea `archive/<CHANGE_NAME>/<CHANGE_NAME>/`.

Mover el contenido de:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/
```

hacia:

```text
spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/
```

Si se usa PowerShell, mover los hijos de la carpeta:

```powershell
Get-ChildItem "spec-ia-agentic-engineer\docs-ia\openspec\changes\<CHANGE_NAME>" | Move-Item -Destination "spec-ia-agentic-engineer\docs-ia\openspec\archive\<CHANGE_NAME>"
```

Despues eliminar la carpeta vacia:

```powershell
Remove-Item "spec-ia-agentic-engineer\docs-ia\openspec\changes\<CHANGE_NAME>"
```

Luego confirmar que:

- `spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/` existe.
- `spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/` ya no existe.
- El contenido completo quedo archivado.

## 5. Cierre

Terminar con:

1. spec estable creada o actualizada
2. carpeta archivada
3. archivos movidos
4. pendientes si quedaron
5. confirmacion de que el change ya no esta en `changes/`
