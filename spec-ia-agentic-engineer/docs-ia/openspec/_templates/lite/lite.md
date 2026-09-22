# Lite: <change-name>

> Version liviana de proposal+design+spec. Usar solo si el change califica como lite
> segun el criterio de `sdd-new-lite.md` / `sdd-new.md` (queda en 1 repo, no rompe
> contratos/esquema existentes, revert simple, no critico). Ante la duda, usar `/sdd-new` (full).

## Por que

Describir el problema, necesidad u oportunidad en 1-2 frases.

## Que cambia

- Cambio 1
- Cambio 2

## Que no cambia

- Fuera de alcance 1

## Contrato tecnico

- Endpoint / comando / pantalla:
- Request / input:
- Response / output:
- Errores:

## Escenarios

- **WHEN** <accion> **THEN** <resultado esperado>
- **WHEN** <caso de error> **THEN** <resultado esperado>

(2-4 bullets sueltos alcanza. Sin el formalismo Requirement/Scenario de `specs/<funcionalidad>/spec.md` —
si el change crece y amerita contrato estable documentado asi, escalar a full.)

## Pruebas previstas

- Unitarias:
- Manual / browser:

## Riesgos y rollback

- Riesgo:
- Rollback: <como revertir, 1-2 lineas>
