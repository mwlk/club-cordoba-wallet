# Documentacion: uc02-listado-credenciales

> **Publicar:** Confluence o Notion (Markdown).
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.
>
> **Estado de este archivo: Aplicado y verificado (2026-09-23).**

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
| Estado | Aplicado y verificado |
| Fecha | 2026-09-23 |
| Responsable | Mirko |

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

Backend (`GetCredentialsQueryHandler`, `GetCredentialByIdQueryHandler`,
`CredentialsController`) sin cambios — ya cumplía el enunciado, verificado
antes de tocar nada.

Frontend, ambos con el mismo mecanismo (`catchError` en el pipe del
`Observable`, en vez de un callback `error` separado en el `subscribe` —
evita duplicar `detectChanges()` en dos branches, un solo code path cubre
éxito y fallo):

- `frontend/src/app/features/credentials/pages/credential-list/credential-list.component.ts`
  — `ngOnInit`: `credentialsService.list().pipe(catchError(() => of([])))`.
  Si el `GET /api/credentials` falla, resuelve a lista vacía; `loaded = true`
  siempre, la pantalla nunca queda en "cargando" indefinido.
- `frontend/src/app/features/credentials/pages/credential-detail/credential-detail.component.ts`
  — `ngOnInit`: `credentialsService.getById(id).pipe(catchError(() => of({success:false,message:'',data:null})))`.
  El `else` ya existente (`notFound = true`) ahora cubre tanto la respuesta
  `success:false` real como cualquier error de red/HTTP (incluido el 404 de
  "no encontrada").

El snackbar global de error (`error.interceptor.ts`) sigue funcionando sin
cambios — el `catchError` de los componentes no lo pisa, corre después en la
cadena (el interceptor ve el error primero, a nivel HTTP).

### Base de datos / persistencia

Sin cambios de esquema ni scripts de BD.

---

## 4. Como probarla

- **Tests unitarios:** `dotnet test tests/ClubCordobaWallet.Tests.Unit` → 15/15 pass.
- **Tests de integración:** `dotnet test tests/ClubCordobaWallet.Tests.Integration` → 3/3 pass, incluye `Get_Credentials_When_Empty_Returns_Empty_List`.
- **Manual (docker-compose):** ver `reports/verificacion-2026-09-23.md`. Resumen:
  1. Backend sano → `/credentials` muestra credenciales reales (control).
  2. Backend apagado → `/credentials` no queda en blanco, muestra estado vacío; `/credentials/{id-inexistente}` no queda en blanco, muestra "Credencial no encontrada".
  3. Backend restaurado, mismo id inexistente (404 real) → sigue mostrando "Credencial no encontrada" sin regresión; `/credentials` vuelve a mostrar los datos reales.

---

## 5. Que impacto tiene

- **Alcance:** solo frontend (`credential-list.component.ts`,
  `credential-detail.component.ts`), sin tocar backend ni contrato HTTP.
- **Riesgo:** bajo — el cambio es puramente defensivo (fallback ante error),
  no altera el camino feliz (verificado sin regresión en control post-fix).
- **Rollback:** revertir los dos archivos frontend a su versión anterior
  (subscribe sin `catchError`); no requiere rollback de datos ni migraciones.

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
