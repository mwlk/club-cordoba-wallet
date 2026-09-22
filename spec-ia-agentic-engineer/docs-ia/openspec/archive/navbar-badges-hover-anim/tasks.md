# Tasks: navbar-badges-hover-anim

## 0. Revision de contexto

- [x] 0.1 Leer `README.MD` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes (`style.css`, `index.html`, changes archivados).
- [x] 0.3 Identificar archivos afectados: `style.css` (principal) y `index.html` (badges + soporte pseudo-elemento).

## 1. Implementacion

### 1.1 Navbar sticky/fixed con z-index 100

- [x] 1.1.1 `.navbar` base: `z-index: 100`; desktop mantiene `position: sticky; top: 0` (sin romper grid del body).
- [x] 1.1.2 Drawer mobile: mantener `position: fixed` y z-index 950 (por encima de searchbar 900) — no bajar su z-index.
- [x] 1.1.3 Verificar que header (1000) sigue por encima y que no hay solapamientos al scroll.

### 1.2 Badge de estado en las cards

- [x] 1.2.1 `:root`: agregar `--badge-new: #0f5132` y `--badge-sale: #8a2f1f` (blanco > 7:1, documentado).
- [x] 1.2.2 CSS `.badge` / `.badge--new` / `.badge--sale`: pill absoluto sobre la imagen, texto blanco, tamano xs, uppercase.
- [x] 1.2.3 `index.html`: agregar `<span class="badge badge--new">Nuevo</span>` y `<span class="badge badge--sale">Oferta</span>` en 4 cards (2 y 2).

### 1.3 Hover de cards (lift + scale)

- [x] 1.3.1 Extender `transition` de la card a `transform` + `box-shadow` (+ border-color existente).
- [x] 1.3.2 Hover: `transform: translateY(-4px) scale(1.02)` + sombra aumentada (patron rgba existente).

### 1.4 Animacion de entrada fadeInUp escalonada

- [x] 1.4.1 `@keyframes fadeInUp` (opacity 0/translateY(16px) -> natural).
- [x] 1.4.2 `animation` en `#catalogo .productos article` con `fill-mode: backwards` y delays por `nth-child`.
- [x] 1.4.3 `@media (prefers-reduced-motion: reduce)`: apagar animacion de entrada y movimiento del hover.

### 1.5 Pseudo-elemento decorativo

- [x] 1.5.1 Segun opcion confirmada: flecha en "Ver detalle" — `::after` con `content: "→"` que se desliza (translateX) en hover.
- [x] 1.5.2 Ajustar `index.html`: se elimino el `&rarr;` literal de los 6 links (la flecha la genera ::after).

### 1.6 Fix post-review (reportes: CTA visible + cursor agrandado)

- [x] 1.6.1 "Ver detalle" convertido en CTA visible (boton `--brand-accent`, texto blanco AAA, `:focus-visible`), con flecha `::after` siempre presente y desliz en hover/focus.
- [x] 1.6.2 `prefers-reduced-motion` tambien neutraliza la transicion del CTA.
- [x] 1.6.3 Cursor agrandado: investigado y descartado del CSS (header/footer/sidebar/botones no usan transform/scale; se reproduce en otras apps). Causa raiz a nivel OS: cursor theme `Breeze_Light` + Chromium sobre Wayland. Resuelto cambiando el cursor theme a `Adwaita` (gsettings + `gtk-3.0/4.0/settings.ini`, size 24).
- [x] 1.6.4 `scale(1.02)` del hover restaurado (no era la causa; se mantiene el efecto lift + grow del spec).

## 2. Validacion

- [x] 2.1 Verificacion estatica: llaves CSS balanceadas (158/158); tokens y reglas nuevas presentes; clases `.badge` usadas en el HTML tienen regla; sin `&rarr;` restantes.
- [x] 2.2 Contraste: `--badge-new` con blanco = 9.36:1 y `--badge-sale` = 8.38:1 (ambos >= 7:1, WCAG AAA); CTA "Ver detalle" blanco sobre `--brand-accent` = 7.92:1 y `--brand-accent-hover` = 9.21:1. Todo documentado en `lite.md`.
- [x] 2.3 Verificacion manual en browser (desktop + mobile) confirmada por el usuario: hover lift+scale OK; resto de escenarios cubierto en `/sdd-review` (navbar al scroll, drawer encima de searchbar, badges, entrada escalonada, reduced-motion estatico).
- [x] 2.4 Confirmados los escenarios de `lite.md` (veredicto `/sdd-review`: PASS WITH GAPS, gap pendiente resuelto).
- [x] 2.5 Cursor agrandado: DESCARTADO del scope del change — causa a nivel OS (cursor theme `Breeze_Light` + Chromium sobre Wayland, se reproduce en otras apps, sin `transform`/`scale` en el CSS del sitio). Mitigado en el entorno (KWin `kcminputrc` + gsettings + `gtk-3.0/4.0/settings.ini` a `Adwaita`, size 24). El `scale(1.02)` del hover se restauro y funciona OK.

## 3. Cierre

- [x] 3.1 No aplica documentacion Confluence/Notion: no se pidio `/sdd-document`; en modo lite `documentation.md` es opcional.
- [x] 3.2 Evaluado: el change no escala a full (solo CSS+HTML en 2 archivos, sin `specs/`).
- [x] 3.3 Archivar con `/sdd-archive` (mover a `openspec/archive/navbar-badges-hover-anim/`).