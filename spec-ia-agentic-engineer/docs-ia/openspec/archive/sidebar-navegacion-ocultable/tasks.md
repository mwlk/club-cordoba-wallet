# Tasks: sidebar-navegacion-ocultable

## 0. Revision de contexto

- [x] 0.1 Leer `README.MD` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes (`style.css`, las 5 paginas `.html`).
- [x] 0.3 Identificar archivos afectados: `style.css` (principal) y ajustes en `index.html`, `producto.html`, `carrito.html`, `contacto.html`, `nosotros.html`.

## 1. Implementacion

### 1.1 Variables CSS en `:root`

- [x] 1.1.1 Definidas 31 variables en `:root`: colores (12), tipografia (6), espaciado (9) y layout/breakpoints (4), todas comentadas/explicadas.
- [x] 1.1.2 Reemplazados los valores magicos de `style.css` por las variables en header, nav, main y footer (estilo propio de cada componente).
- [x] 1.1.3 Paleta y estructura actual preservadas como variables (no cambio el look).

### 1.2 Navbar desktop (logo izquierda, links centrados, boton derecha)

- [x] 1.2.1 Desktop (>=1024px): header+navbar en la misma fila del grid del body — logo a la izquierda (header), links centrados (`flex:1; justify-content:center`) y boton a la derecha (`margin-left:auto`), con `display:flex` + `gap`.
- [x] 1.2.2 Boton "Ingresar" agregado como `<a role="button" class="nav-boton">` en el navbar de las 5 paginas, estilo de boton existente.

### 1.3 Sidebar / drawer mobile ocultable

- [x] 1.3.1 En mobile (<=768px) el `<nav>` se transforma en drawer lateral fijo (`position:fixed`) oculto por defecto y deslizable con `#menu-toggle:checked` (checkbox hack); hamburguesa animada a "X".
- [x] 1.3.2 Drawer con `transform: translateX(-105%)`/`0` + transicion, sin desplazar el contenido (overlay), oculto al volver a tocar la hamburguesa.
- [x] 1.3.3 Eliminada la franja navbar vacia en mobile: el nav sale del flujo como drawer.

### 1.4 Catalogo en grid responsive (1/2/3)

- [x] 1.4.1 `#catalogo .productos` pasado de flex a `display: grid`.
- [x] 1.4.2 Grid responsive con media queries: 1 columna (base/mobile), 2 columnas (`>=600px` tablet) y 3 columnas (`>=1024px` desktop), usando `--bp-tablet`/`--bp-desktop`.
- [x] 1.4.3 Gap y tarjetas ajustados (`--gap-catalogo`, `--padding-tarjeta`).

### 1.5 Responsive mobile/tablet (UX/UI)

- [x] 1.5.1 Espaciados/padding de header, nav, main y footer migrados a variables y revisados en mobile/tablet.
- [x] 1.5.2 Tablet (`>=600px`): catalogo 2 columnas; nav sigue drawer en mobile y barra unificada en desktop.
- [x] 1.5.3 Buscador, sidebar de filtros (`index.html`), tablas y formularios funcionales (sin cambios en su markup).

## 2. Validacion

- [x] 2.1 Verificacion estatica: llaves CSS balanceadas (137/137), 31 variables en `:root`, 5 media queries; HTTP 200 de 5 paginas + `style.css`, `js/main.js`, `favicon.ico` con `python3 -m http.server 8000`.
- [x] 2.2 Verificacion estatica cruzada: todas las clases usadas en los 5 `.html` tienen regla en `style.css`.
- [ ] 2.3 Verificacion visual manual en browser (mobile, tablet, desktop) de las 5 paginas: barra desktop (logo/links/boton), drawer mobile con hamburguesa, catalogo 1/2/3.
- [ ] 2.4 Confirmar los escenarios de `lite.md` cubiertos (variables `:root`, nav flex, drawer mobile, grid 1/2/3, consistencia entre paginas).

## 3. Cierre

- [x] 3.1 Decidido: alcanza con `tasks.md` + `lite.md`; no se corre `/sdd-document` (documentation.md opcional en modo lite).
- [x] 3.2 Evaluado: el change se mantiene lite (no escala a full; no hay `specs/` ni contrato estable que fusionar).
- [x] 3.3 Archivado con `/sdd-archive`.

Pendientes justificados post-archive:
- [ ] 2.3 Verificacion visual manual en browser (mobile, tablet, desktop) — queda a cargo de la persona.
- [ ] 2.4 Confirmar escenarios de `lite.md` — queda a cargo de la persona.