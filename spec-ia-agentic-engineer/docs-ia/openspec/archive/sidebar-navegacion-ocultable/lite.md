# Lite: sidebar-navegacion-ocultable

> Version liviana de proposal+design+spec. Cambio acotado a 1 repo (este),
> no rompe contratos/esquema existentes, revert simple, no critico. Califica como lite.

## Por que

Refactor de la sesion anterior sobre `style.css` para extenderlo y mejorar la
UX/UI en responsive (sobre todo mobile y tablet). Incluye:

1. Un **sidebar/menu ocultable** con la navegacion principal (pedido previo):
   en mobile el menu vive en un **drawer lateral** que se abre/cierra con la
   hamburguesa (checkbox hack, sin JS).
2. **Variables CSS en `:root`** (al menos 6: colores, tipografia, espaciado),
   explicadas, y aplicadas a los componentes de la Clase 1 (header, nav, main,
   footer) que pasan a tener estilo propio.
3. **Nav en desktop** como barra horizontal con logo a la izquierda, links
   centrados y boton a la derecha (`display: flex` + `gap`).
4. **Catalogo en grid** responsive: 1 columna en mobile, 2 en tablet y 3 en
   desktop, definido con media queries.

## Que cambia

- `style.css`:
  - Nueva seccion de **variables en `:root`** con al menos 6 variables
    comentadas y explicadas:
    - Colores: fondo, texto, primario/marca, primario-oscuro, acento/botones,
      neutro/borde y seleccion/hover.
    - Tipografia: familia base y tamanos principales (base, h1/h2/h3, small).
    - Espaciado: unidades de gap/padding (chico, medio, grande) y radio de borde.
  - Header, nav, main y footer pasan a usar las variables (estilo propio de cada
    componente, sin valores magicos).
  - Catalogo pasa de flex a **grid**: `grid-template-columns` con media queries
    (1 columna base/mobile, 2 tablet >=600px, 3 desktop >=1024px).
  - Navbar desktop: fila con logo a la izquierda, links centrados y boton a la
    derecha (`display: flex; gap`), en la barra superior horizontal.
  - Sidebar/drawer mobile: menu de navegacion en panel lateral deslizante,
    oculto por defecto, que se abre con `#menu-toggle:checked ~ .sidebar`
    (checkbox hack) y se cierra con la "X" de la hamburguesa.
  - Ajustes finos de Ux/UI en tablet y mobile (espaciados, paddings).
- Las 5 paginas `.html`: ajustes minimos de estructura si hacen falta para
  acomodar logo/links/boton y el drawer (manteniendo `aria-label` de navegacion).

## Que no cambia

- Contenido y links del menu (Inicio, Catalogo, Nosotros, Carrito, Contacto).
- Secciones/paginas: header de branding, footer con redes, filtros de `index.html`
  (clase `.sidebar` del aside), buscador, producto, carrito, contacto, nosotros.
- No se agrega JS ni frameworks; sigue solo HTML + CSS (checkbox hack).
- No se toca logica de carrito/filtros/Supabase (changes futuros).
- Se mantiene: el catalogo pasa a grid 1/2/3; no a 4 columnas como antes.

## Contrato tecnico

- Pantallas: `index.html`, `producto.html`, `carrito.html`, `contacto.html`, `nosotros.html`.
- Input: estado del checkbox `#menu-toggle` + viewport (mobile/tablet/desktop).
- Output: nav como barra horizontal (desktop) o drawer lateral ocultable (mobile);
  catalogo en grid 1/2/3; estilos tomados de variables `:root`.
- Errores: sin capa de errores; se mantiene accesibilidad (aria-labels,
  `aria-hidden` en checkbox, HTML semantico).

## Escenarios

- **WHEN** se abre una pagina en desktop **THEN** el nav es una barra horizontal
  con logo a la izquierda, links centrados y boton a la derecha (flex + gap).
- **WHEN** se abre una pagina en mobile **THEN** el menu esta oculto en un drawer
  lateral y la hamburguesa lo abre/cierra (sin JS).
- **WHEN** se inspecciona `style.css` **THEN** existe una seccion `:root` con al
  menos 6 variables (colores, tipografia, espaciado) comentadas y aplicadas en
  header/nav/main/footer.
- **WHEN** el catalogo se ve en mobile/tablet/desktop **THEN** muestra 1/2/3
  columnas respectivamente via grid y media queries.
- **WHEN** se navega entre las 5 paginas en desktop y mobile **THEN** nav,
  drawer, grid del catalogo y estilos se comportan igual en todas, sin links rotos.

## Pruebas previstas

- Unitarias: no aplica (proyecto estatico sin suite).
- Manual / browser: servir con `python3 -m http.server 8000`; verificar en las 5
  paginas: barra desktop (logo/links/boton), drawer mobile con hamburguesa,
  catalogo en 1/2/3 columnas, y que filtros/tablas/formularios siguen ok.

## Riesgos y rollback

- Riesgo: refactor de `style.css` completo puede tocar muchas clases; mitigar
  reutilizando los valores/estructura actuales (los colores y breakpoints se
  preservan via variables) y validando pagina por pagina en desktop y mobile.
- Rollback: revert del commit aplicado (cambio acotado, sin migraciones).