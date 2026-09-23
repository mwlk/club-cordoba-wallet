# Lite: infra-karma-test-runner

> Versión liviana. Califica como lite: configuración de tooling (agrega el
> target `test` que faltaba), no toca código de la app ni contratos, revert
> trivial (sacar el target y los 2 archivos nuevos), no crítico.

## Por qué

Detectado al escribir `success.interceptor.spec.ts` (change
`toast-exito-http`): el frontend traía las dependencias de Karma/Jasmine en
`package.json` pero **nunca tuvo el builder de test configurado** —
`ng test` respondía `Cannot determine project or target for command`, sin
`karma.conf.js` ni target `test` en `angular.json`. Gap preexistente del
scaffold, no introducido en esta sesión. El usuario pidió configurarlo para
poder correr los tests escritos.

## Qué cambia

- `frontend/angular.json`: nuevo target `test` (`"builder": "@angular/build:karma"`, el builder moderno basado en esbuild/Vite, consistente con `build`/`serve` que ya lo usan) con `tsConfig`, `karmaConfig`, `browsers: "ChromeHeadlessCI"`, `polyfills: ["zone.js", "zone.js/testing"]` y los mismos `assets`/`styles` que `build`.
- `frontend/tsconfig.spec.json` (nuevo): extiende `tsconfig.json`, `types: ["jasmine"]`, incluye `**/*.spec.ts`.
- `frontend/karma.conf.js` (nuevo): declara el launcher `ChromeHeadlessCI` (`ChromeHeadless` + `--no-sandbox --disable-gpu`, necesario en este sandbox sin Chrome "de verdad" instalado) y hace fallback automático de `CHROME_BIN` a `chromium-browser`/`chromium`/`google-chrome` si no está seteado — para que `ng test` funcione en cualquier entorno sin depender de una variable de entorno manual. Registra explícitamente los plugins `karma-jasmine`/`karma-chrome-launcher` y `frameworks: ['jasmine']` (el builder nuevo no los auto-descubre vía el glob clásico `karma-*`).

## Qué no cambia

- Ningún archivo de producción (componentes, servicios, interceptors).
- Los tests ya escritos (`success.interceptor.spec.ts`) — sólo pasan a poder ejecutarse.

## Contrato técnico

No aplica (tooling, no hay superficie pública).

## Escenarios

- **WHEN** se corre `npm test` (`ng test`) sin flags ni variables de entorno **THEN** levanta Chrome headless y corre toda la suite, terminando (no queda en watch infinito en este setup, ya que no hay Chrome interactivo).
- **WHEN** no está seteado `CHROME_BIN` **THEN** `karma.conf.js` lo resuelve solo si existe `chromium-browser`/`chromium`/`google-chrome` en el sistema.

## Pruebas previstas

- `ng test --no-watch --no-progress` → verde (3/3 con los specs actuales).

## Riesgos y rollback

- Riesgo: mínimo, sólo tooling.
- Rollback: borrar el target `test` de `angular.json` y los 2 archivos nuevos (`tsconfig.spec.json`, `karma.conf.js`).
