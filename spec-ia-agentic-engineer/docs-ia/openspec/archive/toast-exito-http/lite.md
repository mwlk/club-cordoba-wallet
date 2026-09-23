# Lite: toast-exito-http

> Versión liviana de proposal+design+spec. Califica como lite: ajuste de UX
> transversal (un interceptor nuevo, simétrico al ya existente
> `error.interceptor.ts`), no toca backend ni modelo de datos, revert
> trivial (quitar el interceptor de `app.module.ts`), no crítico.

## Por qué

Detectado durante la verificación manual de UC01: sólo hay feedback de error
(`error.interceptor.ts` ya muestra un snackbar en 4xx/5xx). Cuando una
operación (alta de credencial, etc.) sale bien, hoy sólo `credential-create`
arma un snackbar manual propio (`backToList()`), y ningún otro flujo (por
ejemplo, si en el futuro se agrega editar/eliminar) tiene feedback positivo.
El usuario pidió que el interceptor HTTP dispare un toast de éxito genérico,
simétrico al de error, para no depender de que cada componente lo arme a mano.

## Qué cambia

- Nuevo `frontend/src/app/core/interceptors/success.interceptor.ts`
  (`HttpInterceptorFn`, mismo estilo que `error.interceptor.ts`): intercepta
  respuestas de métodos mutantes (`POST`, `PUT`, `DELETE` — no `GET`, listar
  no es una "operación exitosa" que amerite toast) con status 2xx, y muestra
  un `MatSnackBar` usando `response.body.message` (el backend siempre
  devuelve `{success, message, data}` vía el `Result` pattern — se reusa ese
  mensaje real en vez de un texto genérico tipo "Operación exitosa").
- Sólo dispara si `response.body?.success === true` (evita mostrar un toast
  de éxito para respuestas `200` con `success:false`, como "socio no
  encontrado" en la búsqueda, que no es un caso de éxito de escritura ni
  pasa por este filtro de método de todos modos por ser `GET`).
- Registrado junto a los interceptors existentes en `app.module.ts`
  (`provideHttpClient(withInterceptors([loadingInterceptor, errorInterceptor, successInterceptor]))`).
- `credential-create.component.ts#backToList()`: se saca el `MatSnackBar`
  manual (`Credencial emitida para el socio #...`) ya que el interceptor
  ahora cubre ese caso con el mensaje real del backend
  ("Credencial emitida correctamente") — evita mostrar dos toasts para la
  misma acción.

## Qué no cambia

- `error.interceptor.ts` — sigue igual, sin tocar.
- `loading.interceptor.ts` — sigue igual.
- El backend — no se agregan mensajes nuevos, ya devuelve `message` en todos
  los endpoints vía el `Result` pattern.

## Contrato técnico

No aplica (no cambia ningún endpoint ni DTO — es un cambio 100% frontend,
capa de interceptor).

## Escenarios

- **WHEN** un `POST`/`PUT`/`DELETE` responde 2xx con `{success:true, message:"..."}`
  **THEN** aparece un snackbar con ese mensaje.
- **WHEN** un `GET` responde 2xx **THEN** no aparece ningún toast de éxito
  (listar/consultar no es una "operación" que amerite feedback positivo).
- **WHEN** un `POST` responde 2xx pero `success:false` en el body (no debería
  pasar en los endpoints actuales, pero el interceptor no asume nada) **THEN**
  no aparece toast de éxito.
- **WHEN** se emite una credencial exitosamente **THEN** aparece un único
  toast (del interceptor, no dos), con el mensaje real del backend.

## Pruebas previstas

- Manual: alta exitosa → un solo toast, sin duplicados. Alta con firma
  fallando → sigue mostrando sólo el toast de error (interceptor de éxito no
  se dispara en 4xx).
- Unit test simple del interceptor (mock de `HttpHandlerFn` devolviendo una
  respuesta 2xx con `success:true`, verificar que se llama `MatSnackBar.open`
  con el mensaje esperado; y un caso donde no se llama para `GET`).

## Riesgos y rollback

- Riesgo: si algún endpoint futuro devuelve `message` con texto técnico no
  apto para mostrar al usuario final, el toast lo mostraría tal cual — mismo
  riesgo que ya asume `error.interceptor.ts` hoy con `error.error?.message`,
  no es nuevo.
- Rollback: sacar `successInterceptor` de `app.module.ts` y volver el
  snackbar manual en `credential-create.component.ts#backToList()` — cambio
  de 2 archivos, reversible sin tocar backend ni datos.
