# Tasks: css-tokens-bootstrap-html-english

## 0. Revision de contexto

- [x] 0.1 Leer `README.MD`, `spec-ia-agentic-engineer/AGENTS.md`, `PROJECT_CONTEXT.md`, `config.yaml` y el change activo previo.
- [x] 0.2 Inspeccionar `style.css` (variables `:root` actuales y sus usos) y los 5 `.html` (referencias entre paginas).
- [x] 0.3 Identificar archivos afectados: `style.css`, `index.html`, `producto.html`, `carrito.html`, `contacto.html`, `nosotros.html`, `README.MD`, `PROJECT_CONTEXT.md`, `istea-web-agent.md`.

## 1. Implementacion

### 1.1 Variables `:root` en ingles (estilo Bootstrap)

- [x] 1.1.1 `:root` renombrada a set semantico en ingles estilo Bootstrap: brand (primary/secondary/accent + hover), semantic (primary, secondary, success, danger, warning, info, light, dark), grays (100-900), neutros (white, light-hover).
- [x] 1.1.2 Tipografia en ingles: font-family-base, font-size-h1/h2/h3/base/sm/xs, line-height-base.
- [x] 1.1.3 Espaciado en ingles: space-xs/sm/md/lg/xl, card-padding, border-radius-sm/lg, header-height, sidebar-width, content-max-width, catalog-gap, bp-tablet, bp-desktop.
- [x] 1.1.4 Reemplazados TODOS los usos en `style.css`; verificado que no queden variables viejas ni `var()` sin definir.

### 1.2 Renombrar archivos HTML a ingles

- [x] 1.2.1 `git mv producto.html product.html`; `git mv carrito.html cart.html`; `git mv contacto.html contact.html`; `git mv nosotros.html about.html`.
- [x] 1.2.2 Actualizados todos los `href` entre paginas en los 5 `.html` (verificado: solo nombres ingles).

### 1.3 Actualizar referencias en docs

- [x] 1.3.1 `README.MD`: estructura de archivos con nombres ingles.
- [x] 1.3.2 `PROJECT_CONTEXT.md`: estructura y nombres de modulos ingles.
- [x] 1.3.3 `istea-web-agent.md`: nombres de modulos ingles.

## 2. Validacion

- [x] 2.1 Verificacion estatica: sin variables viejas (`--color-*|--espacio-*|--radio-*|--alto-*|--ancho-*|--padding-*|--gap-*`) residuales en `style.css`; sin `var()` sin definir; llaves 137/137.
- [x] 2.2 HTTP 200 de `index.html`, `product.html`, `cart.html`, `contact.html`, `about.html`, `style.css`, `js/main.js`, `favicon.ico` con `python3 -m http.server 8001`.
- [x] 2.3 Cruce estatico: clases/ids usados en los 5 `.html` cubiertos por `style.css`.
- [ ] 2.4 Visual manual en browser (desktop + mobile) que el layout se ve igual que antes del rename.

## 3. Cierre

- [x] 3.1 Decidido: alcanza con `tasks.md` + `lite.md`; no se corre `/sdd-document` (documentation.md opcional en modo lite).
- [x] 3.2 Evaluado: el change se mantiene lite (no escala a full; no hay `specs/` ni contrato estable que fusionar).
- [x] 3.3 Archivado con `/sdd-archive`.

Pendientes justificados post-archive:
- [ ] 2.4 Verificacion visual manual en browser (desktop + mobile) que el layout se ve igual que antes del rename — queda a cargo de la persona.