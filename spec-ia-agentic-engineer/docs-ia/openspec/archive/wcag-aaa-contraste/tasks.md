# Tasks: wcag-aaa-contraste

## 0. Revision de contexto

- [x] 0.1 Leer `README.MD`, `PROJECT_CONTEXT.md`, agente `istea-web-agent.md` y el change previo `css-tokens-bootstrap-html-english`.
- [x] 0.2 Relevar `:root` en `style.css` y medir contraste actual de los pares texto/fondo (links, botones, breadcrumb, descripciones, footer).
- [x] 0.3 Calcular candidatos de color que cumplan >= 7:1 y elegir los que mantienen la paleta (terracota y marrones).

## 1. Implementacion

- [x] 1.1 `--brand-accent` -> `#873a1d` y `--brand-accent-hover` -> `#7a3118` (links y botones AAA).
- [x] 1.2 `--light-hover` -> `#fffdfa` (hovers de links claros y footer sobre marrones).
- [x] 1.3 `--gray-500` -> `#5f4a39` y `--gray-600` -> `#404852` (descripciones y breadcrumb).
- [x] 1.4 `footer p` usa `var(--light-hover)`; reemplazados los `rgba(194, 109, 61...)` por `rgba(163, 87, 47...)`.
- [x] 1.5 Docs (`README.MD`, `PROJECT_CONTEXT.md`, `istea-web-agent.md`): WCAG AAA como requisito.

## 2. Validacion

- [x] 2.1 Contrastes calculados: los 11 pares texto/fondo >= 7:1 (PASS).
- [x] 2.2 Sin hex viejos residuales (`rg`) y llaves balanceadas (137/137).
- [x] 2.3 Render headless Chromium: capturas de las 5 paginas + mobile/tablet en `/tmp/opencode/shots3/`.
- [ ] 2.4 Revision visual manual en browser (desktop + tablet + mobile) por la persona.

## 3. Cierre

- [x] 3.1 Decidido: alcanza con `lite.md` + `tasks.md` (no se corre `/sdd-document`).
- [x] 3.2 Evaluado: change lite (no escala a full; no hay `specs/`).
- [x] 3.3 Archivado con `/sdd-archive`.

Pendientes justificados post-archive:
- [ ] 2.4 Revision visual manual en browser — queda a cargo de la persona.