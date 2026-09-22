# Lite: wcag-aaa-contraste

> Version liviana de proposal+design+spec. Cambio acotado a `style.css` + docs,
> reversible con revert simple, no critica. Califica como lite.

## Por que

Tras el change `css-tokens-bootstrap-html-english` el esquema de color del sitio
(terracota claro `#c26d3d`, hovers, grises de texto) no alcanzaba contraste
accesible. Con el renderer en browser y la calculadora de contraste se detecto
que varios pares texto/fondo no cumplian ni WCAG AA. Se decide endurecer a
**WCAG AAA** (contraste >= 7:1) como requisito del sitio.

## Que cambia

- `style.css` (solo tokens en `:root` y el color de `footer p`; los estilos
  consumen variables, ninguna media query cambia):
  - `--brand-accent`: `#c26d3d` -> `#873a1d` (7.92 pre-over white / 7.24 sobre light)
  - `--brand-accent-hover`: `#8f3f1f` -> `#7a3118` (9.21 / 8.43)
  - `--light-hover`: `#f0d9bd` -> `#fffdfa` (7.37 sobre brand-primary, 10.17 sobre secondary)
  - `--gray-500`: `#6b5542` -> `#5f4a39` (8.32 sobre white)
  - `--gray-600`: `#636b74` -> `#404852` (8.48 sobre light)
  - `footer p`: color `var(--gray-400)` -> `var(--light-hover)`
  - rgba fijos del accent viejo pasado al nuevo: `rgba(163, 87, 47, ...)`
- Docs: `README.MD`, `PROJECT_CONTEXT.md` y el agente `istea-web-agent.md`
  ahora declaran **WCAG AAA** como requisito (>= 7:1) y la regla de no usar
  colores fuera de `:root`.

## Que no cambia

- Layout, estructura HTML ni media queries (los estilos siguen consumiendo variables).
- Contenido ni textos (la UI sigue en espanol).
- No se agrega framework ni build step (sigue HTML/CSS puro).

## Contrato tecnico

- Pantallas: `index.html`, `product.html`, `cart.html`, `contact.html`, `about.html`.
- Input: render de cada pagina en browser (desktop, tablet, mobile).
- Output: todo par texto/fondo con contraste >= 7:1 (WCAG AAA), con los colores
  centralizados en `:root`.
- Errores: sin capa de errores; validacion HTML nativa en formularios.

## Escenarios

- **WHEN** se mide el contraste de links/accent sobre fondo claro **THEN** es >= 7:1.
- **WHEN** se mide white/texto claro sobre `--brand-accent` y hovers **THEN** es >= 7:1.
- **WHEN** se leen breadcrumb, descripciones de cards y texto del footer **THEN** cada par supera 7:1.
- **WHEN** se abren las 5 paginas en browser (desktop/tablet/mobile) **THEN** el layout se mantiene y solo cambia la tonalidad de color.

## Pruebas previstas

- Unitarias: no aplica (proyecto estatico sin suite).
- Manual / browser: calcular contraste de cada par con calculadora (ya ejecutado,
  ver tareas), renderizar capturas en Chromium headless en `/tmp` y revisar
  visualmente desktop + mobile + tablet.

## Riesgos y rollback

- Riesgo: cambiar los tokens puede alterar la vista percibida del sitio; mitigar
  con capturas antes/despues y verificacion visual.
- Riesgo: un rgba hardcodeado con el hex viejo desincroniza los focus rings;
  mitigar reemplazando todos los `rgba(194, 109, 61...)` y validando con `rg`.
- Rollback: revert del commit.