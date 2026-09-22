# Tasks: la-casa-de-los-hilos

## 0. Revision de contexto

- [x] 0.1 Leer `README.MD` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes (`style.css`, `index.html`, `producto.html`, `carrito.html`, `contacto.html`).
- [x] 0.3 Identificar archivos afectados: todos los `.html`, `style.css`, `README.MD`, `PROJECT_CONTEXT.md`, agente `istea-web-agent.md`.

## 1. Implementacion

- [x] 1.1 Actualizar `README.MD` con la arquitectura objetivo (HTML + CSS + JS + Supabase) y la estructura de archivos del sitio.
- [x] 1.2 Actualizar `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` (stack: + JS + Supabase) y el agente `istea-web-agent.md`.
- [x] 1.3 Rebrand del sitio a "La Casa de los Hilos" en `index.html`, `producto.html`, `carrito.html`, `contacto.html` (titulo, meta, header, footer, textos).
- [x] 1.4 Crear `nosotros.html` (seccion quiénes somos) siguiendo el layout comun.
- [x] 1.5 Adaptar `index.html`: landing + catalogo de articulos textiles (hilos, lanas, telas, kits), categorias acordes.
- [x] 1.6 Adaptar `producto.html` a un articulo textil de ejemplo (mantener ficha/especificaciones).
- [x] 1.7 Adaptar `carrito.html` y `contacto.html` al nuevo branding y categorias.
- [x] 1.8 Crear `js/` con `js/main.js` placeholder (sin logica ni cliente Supabase aun) y enlazarlo.
- [x] 1.9 Ajustar `style.css`: paleta/texturas textiles, mantener layout grid/flex, header/navbar/footer y responsive (hamburguesa). No romper estructura actual.
- [x] 1.10 Verificar links de navegacion entre las 5 paginas y anclas del catalogo.

## 2. Validacion

- [x] 2.1 No aplica tests automatizados (proyecto estatico sin suite). Verificacion estatica + HTTP 200 de 5 paginas y assets con `python3 -m http.server 8000`.
- [ ] 2.2 Verificacion visual manual en browser (desktop y mobile) — pendiente de la persona.
- [ ] 2.3 Confirmar los escenarios de `lite.md` cubiertos: branding, hamburguesa responsive, header/navbar/footer consistentes, README con arquitectura.

## Pendientes a futuro (no bloquean este change; salen del scope del esqueleto)

- [ ] F1 Logo real de "La Casa de los Hilos" (el header hoy usa solo texto; `img/logo.png` de Apple fue eliminado).
- [ ] F2 Perfiles reales de redes sociales en el footer (hoy apuntan a la raiz de las plataformas).
- [ ] F3 Logica "Agregar al carrito" en `producto.html` (hoy es anchor placeholder `#`); se resuelve con el change de carrito/Supabase.

## 3. Cierre

- [x] 3.1 Decidido: no correr `/sdd-document`; documentacion suficiente con `tasks.md` y este `lite.md`.
- [x] 3.2 Evaluado: el change permanece lite; no hay `specs/` ni contrato estable que fusionar. El esquema de Supabase y logica de negocio se tratan como changes propios.
- [x] 3.3 Archivado con `/sdd-archive`.

Pendientes justificados post-archive:
- [ ] 2.2 Verificacion visual manual en browser (desktop y mobile) — queda a cargo de la persona.
- [ ] 2.3 Confirmar los escenarios de `lite.md` cubiertos — queda a cargo de la persona.