# Tasks: infra-karma-test-runner

## 0. Revision de contexto

- [x] 0.1 Confirmar que `ng test` no funcionaba (`Cannot determine project or target for command`) y que no existía `karma.conf.js` ni target `test` en `angular.json`.

## 1. Implementacion

- [x] 1.1 Crear `tsconfig.spec.json`.
- [x] 1.2 Agregar target `test` (`@angular/build:karma`) a `angular.json`.
- [x] 1.3 Crear `karma.conf.js` con launcher `ChromeHeadlessCI` (`--no-sandbox --disable-gpu`) y fallback de `CHROME_BIN`.
- [x] 1.4 Registrar explícitamente plugins `karma-jasmine`/`karma-chrome-launcher` y `frameworks: ['jasmine']` (necesario con el builder nuevo).

## 2. Validacion

- [x] 2.1 `ng test --no-watch --no-progress` sin flags manuales → 3/3 en verde.

## 3. Cierre

- [x] 3.1 Archivado con `/sdd-archive`.
