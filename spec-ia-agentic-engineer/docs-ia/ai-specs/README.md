# ai-specs

Material auxiliar para agentes, skills o instrucciones reutilizables.

## Estructura

- `agents/`: agentes especificos del proyecto generados por `spec-setup`.
- `_templates/agent.md`: plantilla para crear agentes de proyecto.
- `skills/`: skills de workflow o conocimiento reusable.

## Agente principal

Despues de ejecutar `spec-setup`, deberia existir un agente principal en:

```text
spec-ia-agentic-engineer/docs-ia/ai-specs/agents/<project>-agent.md
```

Ese archivo traduce la arquitectura real del proyecto a reglas operativas para la IA.
