# Documentacion: <change-name>

> **Publicar:** Confluence o Notion (Markdown). Completar despues de implementar y verificar.
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.

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
| Feature | |
| Ticket / referencia | |
| Estado | Implementado / Parcial / Pendiente |
| Fecha | |
| Responsable | |

---

## 1. Que problema resuelve

Lenguaje de negocio: dolor actual, quien lo sufre y que pasa si no se hace el cambio.

- **Situacion anterior:**
- **Necesidad:**
- **Resultado esperado tras el cambio:**

---

## 2. Como deberia funcionar

Comportamiento observable para usuarios, operacion u otros sistemas. Evitar detalle de clases salvo que sea imprescindible.

- **Flujo principal:**
- **Reglas de negocio:**
- **Casos limite / errores esperados:**
- **Configuracion:** variables, flags, parametros, jobs, permisos, etc.

---

## 3. Que se modifico

Inventario concreto. Usar rutas del repo en texto plano para que sea facil buscar.

### Codigo / aplicacion

| Area | Archivos / componentes | Cambio |
| --- | --- | --- |
| API / UI / Job / Servicio | | |
| Configuracion | | |
| Otros | | |

### Base de datos / persistencia

| Tipo | Nombre | Descripcion |
| --- | --- | --- |
| Tablas / entidades leidas | | |
| Tablas / entidades escritas | | |
| SP / migracion / script | | |
| Datos de referencia | | |

### Scripts de BD o migraciones

Scripts idempotentes cuando sea posible. Indicar ambiente y orden de ejecucion.

```sql
-- Ejemplo: CREATE OR ALTER, migracion, seed, datos de referencia
```

Si no hay cambios de datos o esquema: **Sin cambios de esquema ni scripts de BD.**

---

## 4. Como probarla

| # | Escenario | Pasos | Resultado esperado | Evidencia |
| --- | --- | --- | --- | --- |
| 1 | | | | Test / SQL / log / captura |
| 2 | | | | |

- **Ambiente:**
- **Precondiciones:**
- **Datos de prueba:**
- **Comandos ejecutados:**

**Verificacion tecnica:**

```text
# comandos, queries, logs o pasos manuales
```

- **Evidencia en** `reports/` (si existe):

---

## 5. Que impacto tiene

- **Usuarios:**
- **Operacion / soporte:**
- **Otros modulos o sistemas:**
- **Rendimiento / volumen:**
- **Seguridad / permisos:**
- **Riesgos conocidos:**
- **Rollback / recuperacion:** que se revierte, que no, y pasos concretos

---

## 6. Como mantenerla en el futuro

- **Spec OpenSpec:** `spec-ia-agentic-engineer/docs-ia/openspec/specs/` y `spec-ia-agentic-engineer/docs-ia/openspec/archive/<change-name>/`
- **Config por ambiente:** claves, variables o parametros exactos
- **Monitoreo:** logs, metricas, tablas, dashboards, alertas
- **Deuda / mejoras:**
- **Contacto / dominio:**

---

## Referencias en el repo

- `proposal.md`
- `design.md`
- `specs/<funcionalidad>/spec.md`
- `tasks.md`
- `reports/` si existe

