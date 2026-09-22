# Documentacion: uc02-listado-credenciales

> **Publicar:** Confluence o Notion (Markdown). Completar despues de implementar y verificar.
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.
>
> **Estado de este archivo: PENDIENTE.** Se completa recién en `/sdd-document`,
> después de que `sdd-apply` implemente y verifique las tareas de `tasks.md`.

## Como pegar en Notion

1. Abrir este archivo en el repo y copiar todo el Markdown.
2. En Notion: pagina nueva -> pegar. Notion convierte encabezados, listas y tablas.
3. Si la tabla no se ve bien: menu del bloque -> Turn into -> Table, o Import -> Markdown y elegir el archivo.
4. Los bloques ` ```sql `, ` ```json ` o ` ```text ` deben quedar como Code; si no, crear bloque Code y pegar el contenido.
5. No usar HTML (`<br>`, entidades `&lt;`); usar Markdown compatible con Confluence y Notion.

## Como pegar en Confluence

Pegar como Markdown si el editor lo soporta, o copiar seccion por seccion. Mantener la misma fuente que Notion.

| Campo | Valor |
| --- | --- |
| Feature | Listado de credenciales emitidas (UC02) |
| Ticket / referencia | Prueba técnica — `init.txt`, sección 4.1.1 (UC02) |
| Estado | Pendiente |
| Fecha | — (completar al implementar) |
| Responsable | — |

---

## 1. Que problema resuelve

- **Situacion anterior:** el listado y el detalle ya funcionan según el enunciado, pero si el backend falla, ambas pantallas quedan en blanco sin ningún estado visible (ni carga, ni vacío, ni error) más allá de un snackbar transitorio.
- **Necesidad:** que el estado de la pantalla siempre refleje lo que pasó (datos, vacío, o error/no encontrada).
- **Resultado esperado tras el cambio:** ninguna pantalla de UC02 queda en un estado de carga indefinido.

---

## 2. Como deberia funcionar

- **Flujo principal:** ver `design.md`.
- **Reglas de negocio:** ver `specs/listado-credenciales/spec.md`.
- **Casos limite / errores esperados:** error de red al listar, 404/error al ver detalle.
- **Configuracion:** sin cambios.

---

## 3. Que se modifico

_Pendiente — completar tras `sdd-apply`._

### Base de datos / persistencia

Sin cambios de esquema ni scripts de BD.

---

## 4. Como probarla

_Pendiente — completar con los resultados reales de los tests existentes y la verificación manual (ver `tasks.md` sección 3)._

---

## 5. Que impacto tiene

_Pendiente._

---

## 6. Como mantenerla en el futuro

- **Spec OpenSpec:** `spec-ia-agentic-engineer/docs-ia/openspec/specs/listado-credenciales/spec.md` (tras archivar) y `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc02-listado-credenciales/`.
- **Deuda / mejoras:** ninguna identificada más allá de este change.

---

## Referencias en el repo

- `proposal.md`
- `design.md`
- `specs/listado-credenciales/spec.md`
- `tasks.md`
