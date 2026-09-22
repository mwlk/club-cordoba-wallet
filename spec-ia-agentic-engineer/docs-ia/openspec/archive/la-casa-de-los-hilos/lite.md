# Lite: la-casa-de-los-hilos

> Version liviana de proposal+design+spec. Cambio acotado a 1 repo (este),
> agrega arquitectura y esqueleto sin contratos publicos previos que romper,
> reversible con revert simple, no critico. Califica como lite.

## Por que

El repo parte del template de una tienda de Apple (HTML/CSS puro). El objetivo
academico es un e-commerce de articulos textiles: "La Casa de los Hilos".
Hay que reconfigurar la arquitectura (HTML + CSS + JS + Supabase) y armar el
esqueleto (estructura de archivos y layout header/footer/navbar responsive)
antes de iterar feature por feature con specs.

## Que cambia

- Rebranding del sitio: de "Apple Store" a "La Casa de los Hilos" (articulos textiles).
- Documentar la arquitectura objetivo (HTML + CSS + JS + Supabase) en `README.MD`
  y `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`.
- Definir la estructura de archivos del sitio:
  - `index.html` -> landing + catalogo de articulos textiles (hilos, lanas, telas, kits).
  - `producto.html` -> detalle de articulo con especificaciones.
  - `carrito.html` -> carrito de compras + formulario de compra.
  - `contacto.html` -> formulario de contacto + informacion de la tienda.
  - `nosotros.html` (nueva) -> seccion "quiénes somos".
  - `css/` (style.css), `js/` (iniciamos soporte JS), `img/`.
- Layout base comun en todas las paginas: header con logo/branding, navbar
  (Inicio, Catalogo, Nosotros, Carrito, Contacto), footer con redes, responsive
  con menu hamburguesa en mobile (manteniendo el grid/flex actual).
- Crear `js/` con un `js/main.js` placeholder (sin logica ni cliente de Supabase aun).
  La conexion a Supabase (cliente via CDN o npm) y el esquema de datos se deciden en un change futuro.

## Que no cambia

- Conexion a Supabase (cliente), esquema/tablas ni contrato de datos (change aparte).
- Catalogo: se mantiene estatico en el HTML (como hoy), sin render desde backend.
- Logica de negocio del carrito, buscador, filtros ni contactos (features futuras).
- No se introduce build step ni bundler (HTML/CSS/JS puro).

## Contrato tecnico

- Pantalla: sitio multicapa `index.html`, `producto.html`, `carrito.html`, `contacto.html`, `nosotros.html`.
- Input: navegacion entre paginas y viewport responsive (desktop/tablet/mobile).
- Output: layout comun consistente (header/navbar/footer) y branding "La Casa de los Hilos".
- Errores: sin capa de errores; validacion HTML nativa en formularios por ahora.

## Escenarios

- **WHEN** se abre `index.html` en browser **THEN** se ve header con branding "La Casa de los Hilos", navbar y footer consistentes, con catalogo de articulos textiles.
- **WHEN** se reduce la ventana a < 768px **THEN** aparece el menu hamburguesa y la nav se colapsa (responsive).
- **WHEN** se navega entre `index.html`, `producto.html`, `carrito.html`, `contacto.html` y `nosotros.html` **THEN** header, navbar y footer se mantienen identicos en todas.
- **WHEN** se lee `README.MD` **THEN** documenta la arquitectura objetivo (HTML+CSS+JS+Supabase) y la estructura de archivos del sitio.

## Pruebas previstas

- Unitarias: no aplica (sin suite; proyecto estatico).
- Manual / browser: servir con `python3 -m http.server 8000`; recorrer todas las paginas en desktop y mobile; verificar navbar hamburguesa, branding y que no haya links rotos.

## Riesgos y rollback

- Riesgo: re-empaquetar el layout existente puede tocar muchas clases de `style.css`; mitigar reutilizando la base grid/flex actual y validando pagina por pagina.
- Rollback: revert del commit aplicado; el template Apple queda preservado en git.