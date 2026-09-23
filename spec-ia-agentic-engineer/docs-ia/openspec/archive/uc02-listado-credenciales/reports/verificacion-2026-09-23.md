# Verificación uc02-listado-credenciales — 2026-09-23

## Tests automatizados

- `dotnet test tests/ClubCordobaWallet.Tests.Unit` → 15/15 pass.
- `dotnet test tests/ClubCordobaWallet.Tests.Integration` → 3/3 pass, incluye
  `Get_Credentials_When_Empty_Returns_Empty_List` (evidencia puntual pedida en
  tarea 3.2).

## Verificación manual (docker-compose, `localhost:4200`)

1. Stack arriba (`docker compose up -d --build`), backend sano: `/credentials`
   muestra 2 credenciales reales (Ana García #000007, Docker Compose #000001).
   Control sano antes de romper nada.
2. `docker compose stop api` (backend caído):
   - `/credentials` → NO queda en blanco. Sale del estado "cargando" y muestra
     el estado vacío (`loaded=true`, `credentials=[]` vía
     `catchError(() => of([]))`). Console confirma `ERR_CONNECTION_REFUSED`
     real, no falso positivo.
   - `/credentials/00000000-0000-0000-0000-000000000000` → NO queda en
     blanco. Muestra "Credencial no encontrada" (`notFound=true` vía
     `catchError` en `credential-detail.component.ts`).
3. `docker compose start api` (backend restaurado):
   - Mismo id inexistente (ahora 404 real del backend, no error de red) →
     sigue mostrando "Credencial no encontrada" correctamente (el `else`
     existente ya cubre `success:false`, y el `catchError` cubre el error de
     red — un solo code path para ambos casos, ver `tasks.md` 2.1/2.2).
   - `/credentials` → vuelve a mostrar las 2 credenciales reales, sin
     regresión.

## Resultado

Todos los escenarios de `specs/listado-credenciales/spec.md` verificados:
listado con datos, estado vacío (ya cubierto por integration test, no era
gap), error de red al listar, error de red / no encontrada al ver detalle.
Sin regresiones. Sección 3 de `tasks.md` completa.
