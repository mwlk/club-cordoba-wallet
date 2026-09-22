# docs-ia

Carpeta de documentacion IA dentro de `spec-ia-agentic-engineer/`.

## Contenido

- `spec-ia-agentic-engineer/docs-ia/AI_USAGE.md`: guia de uso con IA.
- `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`: arquitectura y contexto fijo del proyecto donde se copio la carpeta.
- `spec-ia-agentic-engineer/docs-ia/openspec/`: specs, changes, templates y archive.
- `spec-ia-agentic-engineer/docs-ia/ai-specs/`: agentes o skills auxiliares.

## Regla principal

Todo cambio importante debe tener un change en:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/
```

Y antes de archivar debe incluir:

```text
spec-ia-agentic-engineer/docs-ia/openspec/changes/<change-name>/documentation.md
```

Ese archivo es la salida para Confluence o Notion.
