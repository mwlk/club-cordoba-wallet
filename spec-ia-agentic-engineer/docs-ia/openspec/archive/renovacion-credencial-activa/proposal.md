# Proposal: renovacion-credencial-activa

## Por que

El enunciado (`init.txt`) no exige que un socio tenga una única credencial
vigente — cada alta (UC01) simplemente emite una VC nueva. Hoy el sistema
permite emitir credenciales ilimitadas para el mismo DNI: cada alta con un
DNI ya existente reutiliza el `Member` (correcto, ya implementado) pero crea
una `Credential` adicional sin tocar las anteriores, así que un socio puede
terminar con N credenciales "vigentes" simultáneas en el listado (UC02).

Esto no es un bug contra el enunciado, pero sí una debilidad de negocio: una
wallet de credenciales verificables reales normalmente garantiza **una sola
credencial activa por titular**. Se propone esta mejora, fuera del alcance
obligatorio, como valor agregado sobre lo ya entregado (UC01/UC02 archivados).

## Que cambia

- Nuevo endpoint de consulta: dado un DNI, informa si el socio tiene una
  credencial activa (vigente) y hasta cuándo.
- El frontend, antes de confirmar el alta, si el socio ya tiene una
  credencial activa, muestra un popup de confirmación: "¿Generar una nueva
  credencial? La vigente actual quedará vencida desde hoy."
- Si el usuario confirma, el alta expira (setea `validUntil = ahora`) la
  credencial activa anterior del mismo socio, en la misma transacción en la
  que se emite y persiste la nueva.
- Si el usuario cancela, no se envía el alta.

## Que no cambia

- El modelo de datos no agrega columnas: `validUntil` ya vive dentro de
  `vc_json` (JSONB) de cada `Credential`; "expirar" es reescribir ese campo
  dentro del JSON ya persistido, no un nuevo esquema.
- No se toca la canonicalización de la firma del Issuer (sección 4.1.2) ni
  la firma HMAC ya calculada de la credencial vieja — se ajusta un campo del
  JSON ya guardado, no se re-firma nada retroactivamente (ver `design.md`
  para el detalle de por qué esto es aceptable).
- No se implementa revocación real (`credentialStatus`) ni endpoint de
  verificación — fuera de alcance general del proyecto (enunciado 2.2).
- UC01 y UC02 (ya archivados) no se reabren; este change es aditivo sobre
  ellos.

## Funcionalidades

### Nuevas

- `renovacion-credencial-activa`: chequeo de credencial activa por DNI +
  confirmación de renovación en el alta.

### Modificadas

- `POST /api/credentials`: acepta un campo opcional `confirmarRenovacion`
  (bool) y, si el socio tiene una credencial activa y no llega confirmado,
  responde error específico en vez de crear una credencial más.

## Impacto

| Area | Ubicacion | Impacto |
|------|-----------|---------|
| API | `CredentialsController` (nuevo `GET .../active-credential`, `POST /api/credentials` con nuevo campo opcional) | Medio — nuevo endpoint + campo opcional retrocompatible |
| UI | `credential-create` (nuevo popup de confirmación vía `MatDialog`) | Medio |
| Datos | `credentials.vc_json` (update del campo `validUntil` de un registro existente) | Bajo — mismo esquema, sin migración |
| Integraciones | Ninguna | — |
