# Lite: navbar-badges-hover-anim

> Version liviana de proposal+design+spec. Cambio acotado a 1 repo (este), no
> rompe contratos/esquema existentes, revert simple, no critico. Califica como lite.

## Por que

Practica de la materia (prediction `4ff70e2`): se pide darle vida a la navbar y a
las cards del catalogo:

1. **Navbar sticky/fixed** que quede visible al hacer scroll, con `z-index: 100`.
2. **Badge por estado** en las cards: `Nuevo` / `Oferta`. Sin Supabase/backend
   por ahora, el estado va estatico en el HTML; los colores del badge son tokens
   en `:root` calibrados a WCAG AAA.
3. **Hover de las cards**: levante (`translateY(-4px)`) + crecimiento
   (`scale(1.02)`), con `transition` sobre `transform` y `box-shadow`.
   (Nota: el cursor agrandado reportado es un artefacto del cursor
   theme/compositor de Wayland — Breeze_Light + Chromium —, a nivel OS y fuera
   del sitio; ver "Riesgos".)
4. **Animacion de entrada** `@keyframes fadeInUp` en las cards al cargar la
   pagina, con `animation-delay` escalonado por card.
5. **Pseudo-elemento decorativo** (`::before`/`::after`) en un elemento del
   catalogo (candidatos identificados en el mensaje/escenarios).

## Que cambia

- `style.css`:
  - Navbar: z-index base `100`; desktop mantiene `position: sticky; top: 0` (ya
    se comporta como navbar fija al scroll sin romper el grid); el drawer
    mobile mantiene `position: fixed` y su z-index (950) para quedar por encima
    de la searchbar (900). Se documenta decision en "Riesgos".
  - `:root`: dos tokens nuevos de badge con contraste blanco >= 7:1:
    - `--badge-new: #0f5132` (verde oscuro; blanco ~9.4:1).
    - `--badge-sale: #8a2f1f` (terracota oscuro; blanco ~8.4:1).
  - Clases `.badge`, `.badge--new`, `.badge--sale` (pill absoluto sobre la
    imagen de la card).
  - Cards: `transition` extendida a `transform`; hover `translateY(-4px)` +
    `scale(1.02)` + sombra aumentada.
  - `@keyframes fadeInUp` + `animation` escalonada (`nth-child`) con
    `animation-fill-mode: backwards` para no pisar el `transform` del hover al
    terminar; bloque `@media (prefers-reduced-motion: reduce)` que apaga las
    animaciones/transiciones de movimiento.
  - Pseudo-elemento confirmado: **flecha en link "Ver detalle"** — se quita el
    carácter `&rarr;` del HTML y la flecha se emite con `::after`
    (`content: "→"`) deslizándose (translateX) en hover/focus. El link pasa a ser
    un **CTA visible** (botón terracota con texto blanco, contraste AAA) para que
    la acción se distinga a simple vista.
- `index.html`: badges estaticos `<span class="badge badge--new">Nuevo</span>`
  y `badge--sale` en las cards (2 nuevos + 2 ofertas); se elimina el `&rarr;`
  literal de los 6 links "Ver detalle" (lo genera ::after).

## Que no cambia

- Layout general, paleta existente de `:root`, links entre paginas, responsive,
  filtros, sidebar, buscador, resto de paginas (`product.html`, etc.).
- No se agrega JS: animaciones y hamburguesa siguen siendo solo HTML + CSS.
- No se conecta Supabase: el estado del badge es estatico en el HTML.

## Contrato tecnico

- Pantallas: `index.html` (badges + animaciones), `style.css` (todo el estilado).
- Input: viewport/responsive, scroll, hover, carga de pagina,
  `prefers-reduced-motion`.
- Output: navbar visible al scroll (z-index 100), cards con badge de estado,
  hover con lift+scale, entrada fadeInUp escalonada, decoracion via
  pseudo-elemento; todo manteniendo WCAG AAA en texto/fondo.
- Errores: sin capa de errores; accesibilidad: badges son texto real (no solo
  color) y respetan contraste; `prefers-reduced-motion` reduce movimiento.

## Escenarios

- **WHEN** se hace scroll en desktop **THEN** la navbar queda visible arriba con
  `z-index: 100` y el contenido pasa por debajo, sin romper el grid.
- **WHEN** se abre el drawer mobile **THEN** la navbar (fixed) sigue por encima
  de la searchbar y del contenido (z-index 950 mobile).
- **WHEN** se carga `index.html` **THEN** las cards entran con `fadeInUp`
  escalonado (delay creciente por `nth-child`) y, al terminar, el estado
  `transform` vuelve al natural para no pisar el hover.
- **WHEN** el cursor pasa sobre una card **THEN** la card levanta
  (`translateY(-4px)`) y crece (`scale(1.02)`) con transicion suave en
  `transform` y `box-shadow`.
- **WHEN** se mantiene el cursor sobre el link "Ver detalle" (o se navega con
  teclado, `:focus-visible`) **THEN** el CTA cambia a `--brand-accent-hover` y la
  flecha `::after` se desliza (translateX).
- **WHEN** se inspecciona el catalogo **THEN** el "Ver detalle" destaca como boton
  (fondo terracota, texto blanco) y no pasa desapercibido.
- **WHEN** se inspecciona el HTML **THEN** hay cards con badge "Nuevo" y otras
  con "Oferta" (span.badge), y en `:root` existen `--badge-new`/`--badge-sale`.
- **WHEN** se mide contraste **THEN** texto blanco sobre `--badge-new` y
  `--badge-sale` da >= 7:1 (AAA).
- **WHEN** el sistema pide `prefers-reduced-motion: reduce` **THEN** las
  animaciones de entrada y el hover de movimiento se desactivan.
- **WHEN** se usa el pseudo-elemento elegido **THEN** el/los elementos indicados
  muestran el decoro (según opcion confirmada).

## Pruebas previstas

- Unitarias: no aplica (proyecto estatico sin suite).
- Manual / browser: `python3 -m http.server 8000`; verificar en desktop y mobile:
  navbar visible al scroll, drawer encima de la searchbar, badges en las cards
  (6 cards con 4 badgeadas), hover lift+scale, entrada escalonada al recargar,
  contraste de badges AAA, y que `prefers-reduced-motion` apaga las animaciones.

## Riesgos y rollback

- Riesgo z-index: si se baja el navbar desktop a 100, no debe afectar al drawer
  mobile (mantiene 950 sobre searchbar 900) ni al header (1000). No se solapan
  (searchbar pega en `top: var(--header-height)`), se verifica al scroll.
- Riesgo animacion: `animation-fill-mode` que pise el `transform` del hover;
  mitigar con `backwards` (el "to" es el estilo natural de la card).
- Riesgo cursor (DESCARTADO del change; observacion del entorno): el cursor se
  dibujaba agrandado y crecía con hovers repetidos en cualquier elemento. Se
  investigó y confirma causa a nivel OS — cursor theme `Breeze_Light` +
  Chromium/GTK sobre Wayland, se reproduce en otras apps; header/footer/sidebar/
  botones del sitio no usan `transform`/`scale`. No es defecto del sitio: fuera
  del alcance del change. Mitigado fuera del repo (KWin `kcminputrc` + gsettings +
  `gtk-*.0/settings.ini` a cursor theme `Adwaita`, size 24). El `scale(1.02)` del
  hover se mantiene y funciona correctamente.
- Riesgo color: tokens nuevos calibrados (contraste documentado, Blanch > 7:1);
  no se tocan tokens existentes.
- Rollback: revert del commit aplicado (cambio acotado, sin migraciones).