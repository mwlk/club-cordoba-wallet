# sdd-new - Crear cambio OpenSpec

Uso:

```text
/sdd-new CHANGE_NAME="<nombre>" REQUIREMENT="<pedido completo>"
```

`FEATURE_DESCRIPTION` tambien es valido como alias de `REQUIREMENT`.

Si faltan argumentos, pedirlos antes de continuar.

---

## Cuando usar full vs lite

Usar **full** (este comando, 5 archivos: proposal, design, tasks, documentation, specs/spec.md) si se cumple CUALQUIERA:

- Toca mas de un repo o un contrato compartido entre varios repos.
- Es un breaking change: cambia o rompe un contrato publico (API, endpoint, esquema de datos, integracion externa).
- Afecta datos en produccion: escritura/migracion de esquema, backfill, borrado, o cualquier cambio no trivialmente reversible.
- El rollback no es simple (no alcanza con revertir el commit/deploy).
- Es una feature critica para el negocio o con riesgo de seguridad/compliance.

Usar **lite** (`/sdd-new-lite`, 2 archivos: `lite.md`, `tasks.md`) si TODOS estos puntos se cumplen:

- Queda acotada a un solo repo.
- No cambia esquema de datos ni contratos publicos existentes (agrega, no rompe).
- Es reversible con un revert simple del cambio.
- No es critica: si falla, el impacto es bajo y detectable rapido.

Ante la duda, usar full.

---

Estas trabajando en la raiz del proyecto que contiene la carpeta `spec-ia-agentic-engineer`.

## 1. Leer contexto obligatorio

1. `README.md` del proyecto si existe
2. `spec-ia-agentic-engineer/AGENTS.md`
3. `spec-ia-agentic-engineer/CLAUDE.md` si existe
4. `spec-ia-agentic-engineer/docs-ia/README.md`
5. `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`
6. `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`
7. `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md`
8. `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`
9. `spec-ia-agentic-engineer/docs-ia/openspec/specs/README.md`
10. `spec-ia-agentic-engineer/docs-ia/openspec/_templates/`

Si `PROJECT_CONTEXT.md` no existe o esta incompleto, frenar y pedir ejecutar `/sdd-setup` o leer `spec-ia-agentic-engineer/.claude/commands/sdd-setup.md` antes de continuar.
Si no existe agente en `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/`, frenar y pedir ejecutar `/sdd-setup` antes de continuar.

## 2. Nombre del cambio

Usar `CHANGE_NAME` si se proveyo. Si no, derivar un nombre kebab-case desde `REQUIREMENT` o `FEATURE_DESCRIPTION`.

## 3. Crear change

Crear:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/
```

Con:

- `proposal.md`
- `design.md`
- `tasks.md`
- `documentation.md`
- `specs/<funcionalidad>/spec.md`
- `reports/` si aplica

## 4. Reglas

- No implementar codigo en este comando.
- Usar templates de `spec-ia-agentic-engineer/docs-ia/openspec/_templates/`.
- Usar `REQUIREMENT` como descripcion principal de la feature. Si no existe, usar `FEATURE_DESCRIPTION`.
- Usar `PROJECT_CONTEXT.md` como contexto fijo de arquitectura del proyecto.
- Usar el agente de `spec-ia-agentic-engineer/docs-ia/ai-specs/agents/*.md` para ajustar rol, reglas tecnicas y checklist.
- Incluir escenarios WHEN/THEN observables.
- Incluir errores, validaciones, datos, arquitectura y verificacion.
- Incluir cierre documental para Confluence/Notion.

## 5. Cierre

Listar archivos creados y pedir confirmacion humana antes de implementar.


