# Cambios activos

Crear una carpeta por feature:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/
```

Contenido esperado — **full** (`/sdd-new`, para lo grande/critico/multi-repo):

```text
proposal.md
design.md
tasks.md
documentation.md
specs/<funcionalidad>/spec.md
reports/
```

Contenido esperado — **lite** (`/sdd-new-lite`, para lo chico/acotado a 1 repo):

```text
lite.md
tasks.md
reports/
```

`reports/` es opcional y sirve para evidencia de pruebas, SQL, capturas, Postman, logs, etc.
`documentation.md` en modo lite se genera solo bajo demanda con `/sdd-document`, no de entrada.
