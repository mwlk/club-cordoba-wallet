# Design: renovacion-credencial-activa

## Contexto

Flujo actual de alta (`CreateCredentialCommandHandler`):

1. Busca `Member` por DNI (`memberRepository.GetByDniAsync`); si no existe,
   lo crea (sin commit).
2. Arma el `credentialSubject` (`TenantService.BuildSubject`).
3. Invoca al Issuer (`IssuerService.Issue`) — si falla, corta sin persistir
   nada (`IUnitOfWork` ya implementado, ver `docs/decisiones.md`).
4. Crea la `Credential` y hace un único `unitOfWork.SaveChangesAsync`.

`ICredentialRepository` hoy solo tiene `AddAsync`, `GetByIdAsync`,
`GetAllAsync` — no hay forma de saber si un `Member` ya tiene una credencial
vigente, ni de modificar una existente.

`validUntil` no es columna: vive dentro de `vc_json` (JSONB), escrito por
`IssuerService` como `validFrom.AddYears(1)`. Las queries de listado/detalle
ya lo leen parseando el JSON (`GetCredentialsQueryHandler`,
`GetCredentialByIdQueryHandler`).

## Decisiones

- **"Activa" se define como**: existe al menos una `Credential` del `Member`
  cuyo `validUntil` (dentro de `vc_json`) es mayor a `DateTime.UtcNow`. No se
  usa `credentialStatus` (siempre `active`, sin revocación implementada).
- **"Expirar" una credencial vieja = reescribir `validUntil` dentro de su
  `vc_json` ya persistido**, no re-firmarla. Se documenta explícitamente que
  esto invalida la firma HMAC de esa credencial vieja si alguien la
  recalculara — aceptable porque (a) no hay endpoint de verificación de
  firma en este proyecto (fuera de alcance, 2.2), y (b) el propósito es
  puramente de negocio/listado ("no me muestres dos vigentes"), no
  criptográfico. Se anota como limitación conocida en `documentation.md`.
- **`credentialStatus` NO se toca al expirar** (se mantiene `active`/`0` en
  el JSON de la credencial vieja). No se reutiliza `revoked`: esa etiqueta
  es para revocación real, funcionalidad explícitamente fuera de alcance
  (enunciado 2.2), y pisarla acá sería semánticamente incorrecto (el pipe
  `credential-status.pipe.ts` mostraría "Revocada", que no es lo que pasó).
  Ver más abajo ("UI — vigencia visual en UC02") cómo se resuelve que el
  listado muestre "vencida" sin depender de este campo.
- **Confirmación es responsabilidad del backend, no solo UX**: el popup del
  frontend es la UX, pero el backend es quien decide si hace falta
  confirmar. Si `POST /api/credentials` llega para un socio con credencial
  activa y `confirmarRenovacion` no es `true`, el handler corta con
  `Result.Fail("ActiveCredentialExists")`, sin persistir nada. Este `Fail`
  **no lleva `data`** — el `Result<T>` de este comando es
  `Result<CreateCredentialResult>` (`Result.cs` no soporta adjuntar un `T`
  distinto en el camino de error, y `Result<T>.Fail` hoy solo recibe
  `errorKey`, sin tocar esa clase compartida). El front no lo necesita: ya
  tiene el `validUntil` de la consulta GET que hizo antes de mostrar el
  popup. Este 400 solo cubre el caso borde de un cliente que evita el popup
  (curl, otro frontend, o una alta que se coló entre el GET y el POST) — ahí
  alcanza con el mensaje de error genérico de `Errors.resx`, sin dato
  estructurado.
- **Endpoint de consulta separado** (`GET .../active-credential?dni=`) para
  que el frontend pueda mostrar el aviso *antes* de que el usuario aprete
  "Emitir", no solo como reacción a un error del POST. Es también la única
  fuente de la fecha (`validUntil`) que ve el usuario en el popup.
- **Si hay más de una credencial "activa" para el mismo socio** (no debería
  pasar en flujo normal, pero el Riesgo 2 de abajo admite que una carrera
  podría dejar dos), la renovación expira **todas** las que encuentre, no
  solo la más reciente — evita que quede una activa "huérfana" sin que nadie
  se entere.

## Flujo propuesto

```text
GET /api/credentials/members/active-credential?dni=<dni>
  -> buscar Member por dni
  -> si no existe: { success:true, data: null }
  -> si existe: buscar credenciales del member, filtrar validUntil > ahora
  -> si hay alguna: { success:true, data: { credentialId, validUntil } } (la de validUntil mas lejano si hay mas de una)
  -> si no hay ninguna: { success:true, data: null }

POST /api/credentials { ..., confirmarRenovacion?: bool }
  -> resolver/crear Member (igual que hoy)
  -> buscar TODAS las credenciales activas del member (mismo criterio que arriba)
  -> si hay alguna activa Y confirmarRenovacion != true:
       -> Result.Fail("ActiveCredentialExists") (sin data, ver Decisiones)
       -> (nada se persiste, ni Member nuevo si lo hubiera)
  -> si hay alguna activa Y confirmarRenovacion == true:
       -> expirar TODAS las activas encontradas (validUntil = ahora, dentro de cada vc_json)
  -> invocar Issuer (igual que hoy) -> si falla, no se persiste nada
     (ni el Member nuevo, ni la expiracion de las viejas, ni la nueva)
  -> credentialRepository.AddAsync(nueva) + credentialRepository.Update(cada vieja expirada)
  -> unitOfWork.SaveChangesAsync (un solo commit)
```

## Contrato tecnico

- **Endpoint nuevo**: `GET /api/credentials/members/active-credential?dni={dni}`
  - Response 200: `{ success: true, message: null, data: { credentialId: guid, validUntil: string } | null }`
  - `dni` inexistente o socio sin credencial activa -> `data: null` (no es error).
- **Endpoint modificado**: `POST /api/credentials`
  - Request: agrega campo opcional `confirmarRenovacion?: boolean` (default `false` si se omite).
  - Response 201: igual que hoy (`CreateCredentialResult`).
  - Response 400 nuevo caso: `ErrorKey = "ActiveCredentialExists"`, `data: null` — el socio tiene una credencial vigente y no se confirmó la renovación. Sin dato estructurado (ver Decisiones); el front no depende de esto para mostrar el popup, ya lo hizo antes vía el GET.
- **Errors.resx**: nueva clave `ActiveCredentialExists` (mensaje genérico, ej: "El socio ya tiene una credencial vigente. Confirme la renovación para generar una nueva." — sin interpolar fecha, ya que no viaja en `data`).

## UI — vigencia visual en UC02

El listado (`credential-card.component.html`) hoy pinta el estado ("Activa"
verde, dot activo) mirando solo `credential.status === 0`
(`credentialStatus`, siempre `active` — no hay revocación implementada). Si
esta feature solo tocara `validUntil` y no ese campo, la credencial vieja
expirada seguiría mostrándose "Activa" en el listado — gap detectado en
revisión, corregido así:

- `credential-card.component.ts` (detalle expandible de UC02): agregar getter `isExpired` (`new Date(this.credential.validUntil) < new Date()`).
- `credential-list.component.ts` (tile del listado de UC02, **tiene su propio HTML de estado, independiente de `credential-card`** — hay dos lugares a corregir, no uno): agregar método `isExpired(item)` equivalente.
- En ambos templates, el dot/span de estado pasa a considerar `isExpired` ademas de `status`:
  - clase "activo" solo si `status === 0 && !isExpired`.
  - texto: `isExpired ? 'Vencida' : (status | credentialStatus)`.
- No se toca `credentialStatus` en el JSON ni el pipe `credential-status.pipe.ts` (sigue sirviendo para revocada/suspendida si algún día se implementa) — "Vencida" es un estado puramente derivado de la fecha, calculado en cada componente que ya recibe `validUntil`.

## Persistencia

- Tablas / entidades leidas: `members` (por dni), `credentials` (todas las del member, filtradas en memoria por `validUntil` del JSON — igual patrón que `GetCredentialsQueryHandler`, no se agrega columna ni índice).
- Tablas / entidades modificadas: `credentials` — update de `vc_json` de cada credencial expirada (reescritura del campo `validUntil` dentro del JSON, normalmente una sola, ver Decisiones si hubiera más) + insert de la nueva. Se agrega `ICredentialRepository.GetActiveByMemberIdAsync(memberId, ct)` (devuelve `List<Credential>`, no una sola) y `ICredentialRepository.Update(credential)` (sin commit propio, igual patrón que `AddAsync`).
- Transaccion: misma `IUnitOfWork` ya existente — un único `SaveChangesAsync` cubre expirar la vieja + crear la nueva + (si aplica) el `Member` nuevo.
- Concurrencia: no se agrega locking explícito — mismo nivel de exposición a race conditions que el resto del alta hoy (aceptado, fuera de alcance manejar concurrencia — enunciado no lo pide).

## Pruebas

- Unitarias:
  - `CreateCredentialCommandHandlerTests`: socio sin credencial activa -> crea normal, sin tocar nada más. Socio con credencial activa + `confirmarRenovacion=false` -> `Result.Fail("ActiveCredentialExists")` sin `data`, no se persiste nada. Socio con credencial activa + `confirmarRenovacion=true` -> se expira la vieja (`validUntil` actualizado a "ahora" dentro de su JSON) y se crea la nueva, un solo commit. Socio con dos credenciales activas (simulando el escenario del Riesgo 2) + `confirmarRenovacion=true` -> se expiran las dos.
  - Nuevo query handler (`GetActiveCredentialByDniQueryHandler` o similar): sin member -> `null`; member sin credenciales -> `null`; member con credencial vencida -> `null`; member con credencial vigente -> devuelve `credentialId`/`validUntil`.
- Integracion: `CredentialsEndpointTests` — flujo completo: alta -> GET active-credential devuelve la recién creada -> segunda alta mismo DNI sin confirmar -> 400 `ActiveCredentialExists` -> segunda alta con `confirmarRenovacion=true` -> 201, y GET active-credential ahora devuelve la nueva (la vieja ya no cuenta como activa).
- E2E / manual: en el navegador, dar de alta un socio, luego repetir el alta con el mismo DNI (tanto seleccionando el candidato de la lista como tipeándolo completo sin seleccionar) -> debe aparecer el popup de confirmación con la fecha de vigencia actual (la que trajo el GET); confirmar -> nueva credencial emitida, listado (UC02) muestra la vieja con badge "Vencida" (por `isExpired`, no por `credentialStatus`) y la nueva como "Activa".

## Riesgos

- Riesgo 1: reescribir `vc_json` de una credencial ya firmada invalida su firma HMAC si alguien la verificara — mitigado porque no hay verificación implementada en este proyecto; se documenta como limitación conocida, no se oculta.
- Riesgo 2: condición de carrera si dos altas concurrentes para el mismo DNI corren en paralelo (ambas podrían no ver la credencial activa de la otra a tiempo, o el POST llega justo entre el GET y la confirmación del usuario) — riesgo aceptado, mismo nivel que el resto del sistema (sin locking). Mitigado parcialmente: el backend siempre revalida al momento del POST (no confía en lo que vio el GET) y expira todas las activas que encuentre en ese momento, así que el peor caso es que el usuario vea el popup una vez más de lo esperado, nunca que queden dos activas después de confirmar.

## Rollback

Revertir el código del change (`CreateCredentialCommandHandler`, nuevo
endpoint/query, frontend) sin necesidad de rollback de datos: no hay
migración de esquema. Si ya se expiraron credenciales viejas en producción
antes de un rollback, quedarían con `validUntil` en el pasado — dato
correcto según la decisión de negocio tomada, no requiere revertirse.
