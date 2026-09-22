# Documentacion: uc01-alta-credencial

> **Publicar:** Confluence o Notion (Markdown). Completar despues de implementar y verificar.
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.
>
> **Estado de este archivo: PENDIENTE.** Se completa recién en `/sdd-document`,
> después de que `sdd-apply` implemente y verifique las tareas de `tasks.md`.
> No inventar evidencia de pruebas que todavía no corrieron.

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
| Feature | Alta de credencial de socio (UC01) |
| Ticket / referencia | Prueba técnica — `init.txt`, sección 4.1.1 (UC01) |
| Estado | Pendiente |
| Fecha | — (completar al implementar) |
| Responsable | — |

---

## 1. Que problema resuelve

- **Situacion anterior:** el alta de credencial persiste el `Member` nuevo antes de invocar al Issuer; si la firma falla, el `Member` queda en la base aunque el enunciado exige que no se persista nada.
- **Necesidad:** cumplir la extensión 5a de UC01 ("si falla la firma, no se persiste nada") de forma real, no solo documentada.
- **Resultado esperado tras el cambio:** alta y firma atómicas — o se persisten `Member`+`Credential` juntos, o no se persiste ninguno de los dos.

---

## 2. Como deberia funcionar

- **Flujo principal:** ver `design.md` (flujo propuesto con `IUnitOfWork`).
- **Reglas de negocio:** ver escenarios de `specs/alta-credencial/spec.md`.
- **Casos limite / errores esperados:** falla de firma con socio nuevo, falla de firma con socio existente.
- **Configuracion:** sin cambios — sigue dependiendo de `Issuer:HmacKey` (env var / user-secrets).

---

## 3. Que se modifico

_Pendiente — completar tras `sdd-apply` con el listado real de archivos tocados (ver tabla de impacto en `proposal.md` como referencia previa)._

### Base de datos / persistencia

Sin cambios de esquema ni scripts de BD.

---

## 4. Como probarla

_Pendiente — completar con los resultados reales de `dotnet test` y la verificación manual en Angular (ver `tasks.md` sección 3)._

---

## 5. Que impacto tiene

_Pendiente._

---

## 6. Como mantenerla en el futuro

- **Spec OpenSpec:** `spec-ia-agentic-engineer/docs-ia/openspec/specs/alta-credencial/spec.md` (tras archivar) y `spec-ia-agentic-engineer/docs-ia/openspec/archive/uc01-alta-credencial/`.
- **Deuda / mejoras:** condición de carrera preexistente en altas simultáneas con el mismo DNI nuevo (ver `design.md`, sección Riesgos) — no resuelta en este change, fuera del alcance del enunciado.

---

## Referencias en el repo

- `proposal.md`
- `design.md`
- `specs/alta-credencial/spec.md`
- `tasks.md`
