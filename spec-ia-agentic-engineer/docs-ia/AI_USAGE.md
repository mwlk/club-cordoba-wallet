# Guia de uso de Spec IA

Esta guia explica como dejar listo un proyecto para trabajar con IA usando specs antes de tocar codigo.

La idea es simple:

```text
copiar carpeta -> instalar contexto -> crear agente -> crear spec -> revisar spec -> aplicar -> validar -> documentar -> archivar
```

Todo vive dentro de:

```text
spec-ia-agentic-engineer/
```

El instalador no modifica codigo productivo y, por defecto, no crea archivos fuera de esa carpeta.

## 1. Copiar la carpeta

Copiar la carpeta completa `spec-ia-agentic-engineer` dentro del proyecto.

La estructura esperada es:

```text
MiProyecto/
+-- spec-ia-agentic-engineer/
+-- src/
+-- README.md
```

## 2. Configurar contexto base

El instalador `Install-SpecIa.ps1` era para Windows/PowerShell (con separadores `\`) y este proyecto corre en Linux (Fedora) — no se usa.

En su lugar, completar manualmente:

```text
spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
spec-ia-agentic-engineer/docs-ia/openspec/config.yaml
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

con los datos reales del proyecto, o usar el comando `/sdd-setup` (opencode) / `spec-setup` (Cursor).

## 3. Activar comandos

- **opencode**: los comandos slash estan en `.opencode/command/` en la raiz del proyecto — `/sdd-setup`, `/sdd-new`, `/sdd-new-lite`, `/sdd-apply`, `/sdd-review`, `/sdd-document`, `/sdd-archive`.
- **Claude**: comandos en `spec-ia-agentic-engineer/.claude/commands/` (copiar a `.claude/commands/` de la raiz si queres los slash `/sdd-*`).
- **Cursor**: prompts en `spec-ia-agentic-engineer/.cursor/commands/` (run `@.../spec-new.md` etc.).

Correcto:

```text
/sdd-setup
```

Incorrecto:

```text
/sdd-setup.md
```

## 4. Crear el contexto inteligente del proyecto

Este paso se ejecuta una sola vez por proyecto, o cada vez que cambie mucho la arquitectura.

El objetivo es que la IA lea el proyecto y cree:

```text
spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

El archivo `<project>-agent.md` define que rol debe asumir la IA, que reglas tecnicas respetar, que comandos usar y que checklist seguir.

### Cursor

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-setup.md
```

### Claude con slash command

```text
/sdd-setup
```

### Claude sin slash command

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-setup.md y ejecuta ese flujo.
No implementes features. Solo analiza la arquitectura, completa PROJECT_CONTEXT.md y crea el agente del proyecto.
```

## 5. Crear una feature nueva

Este paso no implementa codigo. Solo crea la carpeta del cambio con proposal, design, tasks, spec y documentacion inicial.

Resultado esperado:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/
+-- proposal.md
+-- design.md
+-- tasks.md
+-- documentation.md
+-- specs/<funcionalidad>/spec.md
```

### Cursor

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-new.md
CHANGE_NAME: <nombre-del-cambio>
REQUIREMENT: <descripcion completa de la feature>
```

### Claude con slash command

```text
/sdd-new CHANGE_NAME="<nombre-del-cambio>" REQUIREMENT="<descripcion completa de la feature>"
```

### Claude sin slash command

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-new.md y ejecutalo.
CHANGE_NAME: <nombre-del-cambio>
REQUIREMENT: <descripcion completa de la feature>
```

### Ejemplo real

```text
/sdd-new CHANGE_NAME="punto-entrega-by-id" REQUIREMENT="Nueva query GET by id de punto de entrega. Debe devolver la misma response que GET api/resto/puntos-entrega para un item: id, nombre, descripcion, disponible. idTienda es obligatorio. Debe respetar el acceso por tienda igual que el listado."
```

## 6. Revisar antes de aplicar codigo

Despues de `spec-new`, una persona del equipo debe revisar la carpeta del cambio antes de ejecutar `apply`.

Regla del repo:

```text
sin escenario acordado, no apply
```

### 6.1 proposal.md: por que y alcance

Revisar:

- Por que encaja con el ticket o negocio.
- Que cambia y si el alcance es el esperado.
- Que funcionalidades son nuevas y cuales se modifican.
- Impacto en API, capas, BD y otros modulos.

Si esto esta mal, corregir `proposal.md` antes de mirar la spec.

### 6.2 specs/<funcionalidad>/spec.md: el contrato

Este es el archivo mas importante. Lo usan `apply` y `review`.

Revisar:

- Cada requisito tiene escenarios `WHEN / THEN / AND` observables.
- Los escenarios dicen status HTTP, `Result.Fail`, estado de entidad, filas BD o salida verificable.
- Hay escenarios felices y de error.
- Si es `MODIFIED` o `REMOVED`, no contradice la spec estable sin intencion.
- Nada critico quedo solo en `proposal.md` sin escenario.

### 6.3 design.md: como se va a implementar

Revisar:

- El patron elegido coincide con el README o arquitectura del modulo.
- Las rutas y archivos afectados son creibles.
- No falta controller, handler, task, config, SP, tabla o integracion.
- Incluye decisiones, riesgos y rollback.
- Si hay BD, menciona SP, tablas e idempotencia cuando aplique.

Si `design.md` contradice `spec.md`, ajustar spec o design antes de apply.

### 6.4 tasks.md: plan ejecutable

Revisar:

- La seccion de contexto y validacion refleja lo aprobado.
- Las tareas de implementacion estan troceadas y en orden.
- La verificacion no dice solamente "probar a mano".
- Existen tareas de documentacion y cierre.

Completar o marcar manualmente las tareas de aprobacion cuando el equipo valide la spec.

### 6.5 documentation.md: esqueleto inicial

Despues de `new`, `documentation.md` puede quedar como esqueleto.

No es gate para `apply`.

Se completa al final de `apply` o con `document`.

## 7. Checklist corta post-new / pre-apply

| Pregunta | Donde mirar |
| --- | --- |
| Resuelve el ticket correcto? | `proposal.md` |
| Se puede revisar escenario por escenario? | `specs/<funcionalidad>/spec.md` |
| Arquitectura y BD estan bien? | `design.md` |
| Falta Postman, SQL, permisos o integracion? | `proposal.md`, `design.md`, `tasks.md` |
| El nombre del cambio es claro? | `changes/<CHANGE_NAME>/` |

## 8. Implementar

Ejecutar este paso solo cuando el equipo aprueba la spec.

### Cursor

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-apply.md
CHANGE_NAME: <nombre-del-cambio>
```

### Claude con slash command

```text
/sdd-apply CHANGE_NAME="<nombre-del-cambio>"
```

### Claude sin slash command

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-apply.md y ejecutalo.
CHANGE_NAME: <nombre-del-cambio>
```

## 9. Validar implementacion contra la spec

Este paso revisa si el codigo cumple el contrato acordado.

### Cursor

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-review.md
CHANGE_NAME: <nombre-del-cambio>
```

### Claude con slash command

```text
/sdd-review CHANGE_NAME="<nombre-del-cambio>"
```

### Claude sin slash command

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-review.md y ejecutalo.
CHANGE_NAME: <nombre-del-cambio>
```

## 10. Documentar para Confluence o Notion

Este paso completa `documentation.md` con lo necesario para documentacion interna.

### Cursor

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-document.md
CHANGE_NAME: <nombre-del-cambio>
```

### Claude con slash command

```text
/sdd-document CHANGE_NAME="<nombre-del-cambio>"
```

### Claude sin slash command

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-document.md y ejecutalo.
CHANGE_NAME: <nombre-del-cambio>
```

## 11. Que hace la persona despues de aprobar

1. Corrige la carpeta del cambio si algo esta mal, sin tocar codigo productivo.
2. Ejecuta `spec-apply` o `/sdd-apply`.
3. Ejecuta `spec-review` o `/sdd-review`.
4. Resuelve gaps que cambien el contrato final.
5. Ejecuta `spec-document` o `/sdd-document`.
6. Ejecuta `spec-archive` o `/sdd-archive`.

## 12. Archivar el cambio

Archive es el cierre formal del change.

Sirve para dos cosas:

1. Dejar una spec estable en `spec-ia-agentic-engineer/docs-ia/openspec/specs/<funcionalidad>/spec.md`.
2. Guardar el historial completo del cambio en `spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/`.

La spec estable es la fuente de verdad para futuros cambios. Si alguien modifica esa funcionalidad mas adelante, debe leer esa spec para saber que comportamiento existe y que no puede romper.

La carpeta archivada guarda el historial completo del cambio: `proposal.md`, `design.md`, `tasks.md`, `documentation.md`, specs y evidencia. Sirve para responder despues por que se implemento asi, que se descarto, que se aprobo y que riesgos quedaron documentados.

Se usa cuando:

- El codigo fue implementado.
- El cambio ya esta en produccion o no tiene mas iteraciones pendientes.
- La spec fue revisada contra la implementacion.
- `documentation.md` quedo lista para Confluence o Notion, o con pendientes explicitos.
- Las tareas de validacion y documentacion estan cerradas antes de archive.
- Las tareas de `tasks.md` estan completas o justificadas.
- La evidencia de pruebas esta documentada en `documentation.md`, `tasks.md` o `reports/`.

No archivar si:

- Falta aprobar la spec.
- Falta implementar codigo.
- Falta documentacion final.
- Hay escenarios importantes sin revisar.
- El review dejo gaps abiertos que cambian el contrato final.
- No esta claro si el contrato estable debe actualizarse.

Ejemplo: si el review dejo un `WARNING` sobre una validacion `[Required]`, conviene resolverlo antes de archivar. Si se archiva antes, la spec estable puede quedar describiendo un comportamiento que todavia no es el estado final real.

### 12.1 Integrar contrato estable

Antes de mover el cambio a archive, revisar si el delta de:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/specs/<funcionalidad>/spec.md
```

debe fusionarse en:

```text
spec-ia-agentic-engineer/docs-ia/openspec/specs/<funcionalidad>/spec.md
```

Regla practica:

- Si la feature agrega o modifica comportamiento permanente, actualizar la spec estable.
- Si fue una prueba, spike o cambio descartado, no fusionar como contrato vigente.
- Si hubo `MODIFIED` o `REMOVED`, dejar el contrato estable reflejando el estado final real.

Ejemplo:

```text
spec-ia-agentic-engineer/docs-ia/openspec/specs/pending-notifications/spec.md
```

Ese archivo deberia decir cual es el contrato vigente de `pending-notifications`. Si en tres meses alguien toca esa funcionalidad, no deberia necesitar leer el diff de git del cambio original para saber que comportamiento debe respetar.

### 12.2 Mover el change a archive

Mover:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<CHANGE_NAME>/
```

a:

```text
spec-ia-agentic-engineer/docs-ia/openspec/archive/<CHANGE_NAME>/
```

Debe conservarse completo:

```text
proposal.md
design.md
tasks.md
documentation.md
specs/
reports/ si existe
```

### 12.3 Ejecutar archive

Antes de ejecutar archive, el orden correcto es:

```text
validar -> documentar -> archivar
```

El comando de archive debe actualizar `tasks.md` antes de mover la carpeta:

- marcar la validacion/documentacion como cerrada si corresponde.
- marcar la integracion de spec estable como completa o justificar que no aplica.
- marcar la tarea de mover a archive solo despues de mover correctamente.

En Cursor:

```text
Run @spec-ia-agentic-engineer/.cursor/commands/spec-archive.md
CHANGE_NAME: <nombre-del-cambio>
```

En Claude con slash command:

```text
/sdd-archive CHANGE_NAME="<nombre-del-cambio>"
```

En Claude sin slash command:

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-archive.md y ejecutalo.
CHANGE_NAME: <nombre-del-cambio>
```

### 12.4 Checklist antes de archivar

| Pregunta | Donde mirar |
| --- | --- |
| La implementacion paso review? | salida de `spec-review` o `/sdd-review` |
| La documentacion esta completa? | `documentation.md` |
| Las tareas estan cerradas o justificadas? | `tasks.md` |
| Hay evidencia de pruebas? | `documentation.md`, `reports/`, `tasks.md` |
| La spec estable quedo actualizada? | `openspec/specs/<funcionalidad>/spec.md` |
| El change ya no esta activo? | `openspec/archive/<CHANGE_NAME>/` |

## Resumen en una frase

Primero se acuerda el contrato en `spec.md`; recien despues la IA implementa siguiendo ese contrato, el contexto del proyecto y el agente especializado.

## Changes, specs y archive

Hay tres lugares con archivos `spec.md`, pero cada uno tiene un proposito distinto.

### 1. `changes/<change-name>/specs/<funcionalidad>/spec.md`

Es el borrador del contrato.

Se crea con `spec-new` o `/sdd-new`.

Describe que va a cambiar, solo como delta:

- `ADDED`: lo que se agrega.
- `MODIFIED`: lo que se modifica.
- `REMOVED`: lo que se elimina.

Es temporal. Vive mientras el change esta activo. Cuando el change cierra, sale de `changes/` y pasa a `archive/`.

Ejemplo:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/pending-notifications/specs/pending-notifications/spec.md
```

### 2. `specs/<funcionalidad>/spec.md`

Es el contrato vigente.

Es la fuente de verdad permanente del sistema.

Se actualiza al archivar: el delta del change se fusiona aca.

Si en tres meses alguien toca `pending-notifications`, lee este archivo para saber que comportamiento existe y que no puede romper.

No depende de ningun change particular.

Ejemplo:

```text
spec-ia-agentic-engineer/docs-ia/openspec/specs/pending-notifications/spec.md
```

### 3. `archive/<change-name>/specs/<funcionalidad>/spec.md`

Es el delta historico congelado.

Es exactamente el mismo archivo que estaba en `changes/` antes de archivar.

Se guarda para responder despues:

- que se acordo en ese change.
- que se descarto.
- por que se implemento asi.
- que riesgos o pendientes quedaron documentados.

No deberia modificarse mas.

Ejemplo:

```text
spec-ia-agentic-engineer/docs-ia/openspec/archive/pending-notifications/specs/pending-notifications/spec.md
```

### Regla practica

Cuando llega algo nuevo:

1. Se lee `specs/<funcionalidad>/spec.md` para entender lo vigente.
2. Se crea un change nuevo en `changes/<change-name>/`.
3. Se trabaja sobre el delta del change.
4. Al terminar, `archive` fusiona el resultado en `specs/`.
5. El change completo pasa a `archive/` como historial.

En corto:

```text
changes/ = lo nuevo en curso
specs/ = lo ultimo aprobado/oficial
archive/ = historial cerrado del cambio
```

## Prueba rapida en un proyecto nuevo

Crear proyecto de prueba:

```powershell
$proyecto = Join-Path $env:TEMP "ProyectoPruebaSpecIa"
mkdir $proyecto
cd $proyecto
mkdir src
Set-Content README.md "# Proyecto prueba Spec IA"
```

Copiar dentro la carpeta (dondequiera que esté ubicada la plantilla):

```powershell
Copy-Item spec-ia-agentic-engineer $proyecto -Recurse
```

Completar el contexto base con el comando de setup (opencode `/sdd-setup`, Cursor `spec-setup`, o manual: `PROJECT_CONTEXT.md` + `config.yaml` + agente en `ai-specs/agents/`).

Verificar:

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

Luego probar:

```text
/sdd-setup
/sdd-new CHANGE_NAME="prueba-listar-items" REQUIREMENT="Crear una pantalla o endpoint de prueba que liste items de ejemplo. No implementar codigo todavia."
```

Resultado esperado:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/prueba-listar-items/
```
