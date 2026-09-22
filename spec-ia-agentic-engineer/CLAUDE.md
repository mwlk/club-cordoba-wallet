# CLAUDE

Usar este archivo como punto de entrada para Claude cuando la carpeta `spec-ia-agentic-engineer/` esta copiada dentro de un proyecto.

## Configuracion inicial

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-setup.md y ejecuta ese flujo.
No implementes features. Solo completa el contexto base del proyecto.
```

## Nueva feature

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-new.md.
CHANGE_NAME: <change-name>
REQUIREMENT: <descripcion de la feature>
```

## Implementar

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-apply.md.
CHANGE_NAME: <change-name>
```

## Documentar

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-document.md.
CHANGE_NAME: <change-name>
```

## Revisar

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-review.md.
CHANGE_NAME: <change-name>
```

## Archivar

```text
Lee spec-ia-agentic-engineer/.claude/commands/sdd-archive.md.
CHANGE_NAME: <change-name>
```

Si los slash commands fueron copiados a `.claude/commands` en la raiz del proyecto, tambien se pueden usar directamente como `/sdd-new`, `/sdd-apply`, `/sdd-review`, `/sdd-document` y `/sdd-archive`.
