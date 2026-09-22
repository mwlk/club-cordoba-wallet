# Lite: css-tokens-bootstrap-html-english

> Version liviana de proposal+design+spec. Cambio acotado a 1 repo (este),
> reversibles con revert simple, no critica. Califica como lite.

## Por que

El `style.css` actual define sus variables con nombres en espanol sin seguir un
esquema semantico clasico, y los archivos HTML usan nombres en espanol
(`producto.html`, `carrito.html`, `contacto.html`, `nosotros.html`). Se pide:

1. Renombrar las variables de `:root` a ingles, al estilo Bootstrap (primary,
   secondary, success, danger, warning, info, light, dark + neutros/marca).
2. Renombrar los archivos HTML a ingles y actualizar todas las referencias.

## Que cambia

- `style.css`: variables `:root` renombradas a un set semantico completo en
  ingles estilo Bootstrap (colores + tipografia + espaciado + layout), con los
  mismos valores actuales preservados (no cambia el look). Se actualizan todos
  los usos dentro del CSS.
- Archivos renombrados:
  - `producto.html` -> `product.html`
  - `carrito.html` -> `cart.html`
  - `contacto.html` -> `contact.html`
  - `nosotros.html` -> `about.html`
  - `index.html` se mantiene (ya es ingles).
- Actualizacion de todas las referencias (href/links) en los 5 `.html`, y de la
  estructura de archivos en `README.MD`, `PROJECT_CONTEXT.md` y el agente
  `istea-web-agent.md`.

## Que no cambia

- Contenido, textos ni layout del sitio (se mantiene en espanol la UI).
- Semantica ni escenarios del change anterior (`sidebar-navegacion-ocultable`).
- No se agrega framework JS ni build step (sigue HTML/CSS puro).

## Contrato tecnico

- Pantallas: `index.html`, `product.html`, `cart.html`, `contact.html`, `about.html`.
- Input: navegacion entre paginas y viewport responsive (desktop/tablet/mobile).
- Output: mismas pantallas con nombres de archivos ingles; `:root` expone
  variables semanticas en ingles estilo Bootstrap.
- Errores: sin capa de errores; validacion HTML nativa en formularios.

## Escenarios

- **WHEN** se navega entre las 5 paginas **THEN** todos los links apuntan a los nombres ingles y no hay 404.
- **WHEN** se abre `style.css` y se inspecciona `:root` **THEN** las variables se nombran en ingles (primary, secondary, success, danger, warning, info, light, dark, etc.) y el sitio se ve igual que antes.
- **WHEN** se sirven `product.html`, `cart.html`, `contact.html` y `about.html` **THEN** devuelven HTTP 200.
- **WHEN** se lee `README.MD` **THEN** la estructura de archivos refleja los nombres ingles.

## Pruebas previstas

- Unitarias: no aplica (proyecto estatico sin suite).
- Manual / browser: servir con `python3 -m http.server 8000`; recorrer todas las
  paginas (nombres ingles), verificar 200 y que el layout se mantenga identico.

## Riesgos y rollback

- Riesgo: renombrar archivos puede dejar referencias rotas; mitigar actualizando
  todos los href y validando con HTTP 200 pagina por pagina.
- Riesgo: renombrar variables puede desincronizar el CSS si queda un uso viejo;
  mitigar con reemplazo integro de la tabla de variables y revision estatica.
- Rollback: revert del commit; `git mv` conserva el historial.