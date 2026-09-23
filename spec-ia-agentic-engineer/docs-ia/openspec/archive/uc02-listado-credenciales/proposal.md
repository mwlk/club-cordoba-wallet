# Proposal: uc02-listado-credenciales

## Por que

El enunciado (`init.txt`, UC02) exige listar las credenciales del tenant con
foto, nombre, apellido, categoría, número de socio, vigencia y estado,
incluyendo el estado vacío cuando no hay ninguna. El scaffold actual ya
implementa esto correctamente: `GetCredentialsQueryHandler` devuelve lista
vacía sin credenciales, y `CredentialsEndpointTests.Get_Credentials_When_Empty_Returns_Empty_List`
(verificado en el código, `backend/tests/ClubCordobaWallet.Tests.Integration/Api/CredentialsEndpointTests.cs:41`)
ya cubre ese escenario con un test real. `credential-list.component.html`
también renderiza `<app-empty-state>` cuando `loaded && credentials.length === 0`.

Se encontraron dos gaps de UX del mismo tipo, ambos en `subscribe()` sin
callback de `error` (verificado en el código):

- `credential-list.component.ts#ngOnInit` — si `GET /api/credentials` falla,
  `loaded` nunca pasa a `true` y la pantalla queda en blanco (ni lista, ni
  estado vacío, ni error visible más que el snackbar global).
- `credential-detail.component.ts#ngOnInit` — el controller responde `404`
  (`NotFound`) tanto para "credencial no encontrada" como para cualquier otro
  error; al ser un `HttpErrorResponse`, el `subscribe` sin callback de error
  nunca ejecuta el bloque que setea `notFound = true`, así que la pantalla de
  detalle queda en blanco en vez de mostrar el mensaje de "no encontrada".

Este change formaliza UC02 como spec propia (trazabilidad, criterio del
usuario de "cada caso de uso debe ser un spec") y corrige esos dos gaps de UX.

## Que cambia

- Se documenta UC02 como spec estable con escenarios WHEN/THEN verificables.
- `credential-list.component.ts#ngOnInit` agrega callback de `error` al
  `subscribe` para setear `loaded = true` (con lista vacía) y así no dejar la
  pantalla en blanco tras un error de red; el snackbar global ya informa el
  mensaje.
- `credential-detail.component.ts#ngOnInit` agrega callback de `error` al
  `subscribe` para setear `notFound = true` cuando el `GET` devuelve 404 (u
  otro error HTTP), en vez de dejar la pantalla en blanco.

## Que no cambia

- `GetCredentialsQueryHandler`, `GetCredentialByIdQueryHandler`: sin cambios
  de comportamiento (ya cumplen el enunciado, incluido el estado vacío, con
  test de integración existente).
- El contrato HTTP de `GET /api/credentials` y `GET /api/credentials/{id}`.

## Funcionalidades

### Nuevas

- `listado-credenciales` (UC02 completo, formalizado como spec estable)

### Modificadas

- `credential-list.component.ts` (maneja error de `list()`)
- `credential-detail.component.ts` (maneja error de `getById()`)

## Impacto

| Area | Ubicacion | Impacto |
|------|-----------|---------|
| API | `backend/src/ClubCordobaWallet.Api/Controllers/CredentialsController.cs` (`List`, `GetById`) | Ninguno — ya cumple el enunciado |
| Aplicación | `Application/Credentials/Queries/GetCredentials/`, `Queries/GetCredentialById/` | Ninguno — ya cumplen, se referencian en la spec |
| UI | `frontend/src/app/features/credentials/pages/credential-list/credential-list.component.ts`, `credential-detail/credential-detail.component.ts` | Agregan manejo de error en `ngOnInit` |
| Datos | `credentials`, `members` (solo lectura) | Sin cambios |
| Integraciones | Ninguna | — |
