# Tasks: toast-exito-http

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto y `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md`.
- [x] 0.2 Inspeccionar `error.interceptor.ts` y `loading.interceptor.ts` (patrón a seguir).
- [x] 0.3 Identificar archivos afectados (ver `lite.md`).

## 1. Implementacion

- [x] 1.1 Crear `success.interceptor.ts`: filtra por método (`POST`/`PUT`/`DELETE`), respuesta 2xx y `body.success === true`; dispara `MatSnackBar.open(body.message, 'Cerrar', {duration: 4000, panelClass: 'cc-snack-success'})`.
- [x] 1.2 Registrar en `app.module.ts` junto a `loadingInterceptor`/`errorInterceptor`.
- [x] 1.3 Sacar el snackbar manual de `credential-create.component.ts#backToList()` (evitar doble toast) — de paso se pudo sacar la dependencia a `MatSnackBar` del componente entero.
- [x] 1.4 Mantenido estilo del proyecto (interceptor funcional `HttpInterceptorFn`, sin clases).

## 2. Validacion

- [x] 2.1 Escrito `success.interceptor.spec.ts` (3 casos: POST 2xx+success → toast con el mensaje real; GET → no dispara; POST 2xx+success:false → no dispara). El scaffold nunca tuvo el test runner configurado (`ng test` fallaba con "Cannot determine project or target for command") — se agregó el target `test` (`@angular/build:karma`) a `angular.json`, `tsconfig.spec.json` y `karma.conf.js` (ver `infra-karma-test-runner`, change aparte). **3/3 tests en verde** (`ng test --no-watch`).
- [x] 2.2 Verificación manual: alta exitosa muestra un solo toast ("Credencial emitida correctamente", tomado del backend) apenas responde el POST; al volver al listado no aparece un segundo toast.
- [x] 2.3 Escenarios de `lite.md` cubiertos (2xx+success → toast; GET → sin toast).

## 3. Cierre

- [x] 3.1 Archivado con `/sdd-archive`.
