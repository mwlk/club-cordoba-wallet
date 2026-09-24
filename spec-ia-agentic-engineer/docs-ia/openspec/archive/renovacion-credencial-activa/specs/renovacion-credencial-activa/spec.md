# Delta Spec: renovacion-credencial-activa

## ADDED Requirements

### Requirement: Consulta de credencial activa por DNI

El sistema DEBE exponer un endpoint que, dado un DNI, informe si el socio
asociado tiene una credencial vigente (`validUntil` mayor a la fecha/hora
actual) y, si la tiene, hasta cuándo.

#### Scenario: Socio sin credencial activa

- **WHEN** se consulta `GET /api/credentials/members/active-credential?dni={dni}` para un DNI de un socio sin credenciales, o con todas vencidas, o de un DNI inexistente
- **THEN** la respuesta es `200 OK` con `data: null`

#### Scenario: Socio con credencial activa

- **WHEN** se consulta el mismo endpoint para un DNI de un socio con al menos una credencial cuyo `validUntil` es posterior a ahora
- **THEN** la respuesta es `200 OK` con `data: { credentialId, validUntil }` de esa credencial

### Requirement: Confirmación obligatoria para renovar credencial activa

El sistema DEBE impedir que se emita una nueva credencial para un socio que
ya tiene una credencial vigente, salvo que la operación llegue marcada
explícitamente como confirmada.

#### Scenario: Alta sin confirmar para socio con credencial activa

- **WHEN** se hace `POST /api/credentials` con el DNI de un socio que tiene una credencial vigente
- **AND** el campo `confirmarRenovacion` no está presente o es `false`
- **THEN** la respuesta es un error (`success: false`, `ErrorKey: "ActiveCredentialExists"`, `data: null`) — sin fecha estructurada; el cliente que necesite mostrarla ya la obtuvo antes con el endpoint de consulta
- **AND** no se persiste ninguna credencial ni socio nuevo

#### Scenario: Alta confirmada para socio con credencial activa

- **WHEN** se hace `POST /api/credentials` con el DNI de un socio que tiene una credencial vigente
- **AND** el campo `confirmarRenovacion` es `true`
- **THEN** todas las credenciales vigentes anteriores de ese socio quedan con `validUntil` igual al momento de la operación (dejan de estar vigentes)
- **AND** se emite y persiste una nueva credencial con la vigencia normal (`validFrom` = ahora, `validUntil` = ahora + 1 año)
- **AND** todos los cambios (expirar la(s) vieja(s), crear la nueva) se persisten en una única transacción

#### Scenario: Alta para socio sin credencial activa

- **WHEN** se hace `POST /api/credentials` con el DNI de un socio nuevo, o de un socio existente sin ninguna credencial vigente
- **THEN** el comportamiento es el mismo que hoy (sin cambios): se emite y persiste la nueva credencial, sin tocar credenciales anteriores

#### Scenario: Falla de firma durante una renovación confirmada

- **WHEN** se hace `POST /api/credentials` con `confirmarRenovacion: true` para un socio con credencial activa
- **AND** el Issuer falla al firmar la nueva credencial
- **THEN** no se persiste nada: ni la nueva credencial, ni el `Member` nuevo si aplicaba, ni el `validUntil` actualizado de la credencial vieja (sigue vigente como antes del intento)

### Requirement: Popup de confirmación en el frontend antes de renovar

El frontend, al detectar que el DNI cargado en el formulario (sin importar
si llegó eligiendo un candidato de la búsqueda o tipeándolo directamente) ya
tiene una credencial vigente, DEBE mostrar un popup de confirmación antes de
enviar el alta, y DEBE permitir cancelar sin efectos. El chequeo se hace
sobre el valor del campo DNI del formulario al momento de enviar, no sobre
si el usuario llegó a seleccionar un candidato de la lista de búsqueda —
ambos caminos deben quedar cubiertos.

#### Scenario: Popup se muestra al detectar credencial activa (socio seleccionado de la búsqueda)

- **WHEN** el usuario selecciona un candidato de la búsqueda de socio cuyo DNI tiene una credencial vigente
- **AND** intenta enviar el formulario de alta
- **THEN** se muestra un popup de confirmación indicando la fecha de vigencia actual y preguntando si desea generar una nueva credencial

#### Scenario: Popup se muestra al detectar credencial activa (DNI tipeado sin seleccionar candidato)

- **WHEN** el usuario tipea manualmente el DNI completo de un socio con credencial vigente, sin clickear ningún candidato de la lista de búsqueda
- **AND** intenta enviar el formulario de alta
- **THEN** se muestra el mismo popup de confirmación que en el caso de selección por búsqueda

#### Scenario: Usuario confirma la renovación

- **WHEN** el usuario confirma el popup
- **THEN** el formulario envía `POST /api/credentials` con `confirmarRenovacion: true`
- **AND**, si la respuesta es exitosa, se muestra el resultado normal del alta (igual que hoy)

#### Scenario: Usuario cancela la renovación

- **WHEN** el usuario cancela el popup
- **THEN** no se envía ningún request
- **AND** el formulario queda como estaba, sin marcar `submitting`

### Requirement: Estado visual "Vencida" en el listado (UC02)

El listado de credenciales DEBE mostrar como vencida a una credencial cuyo
`validUntil` ya pasó, aunque su `credentialStatus` almacenado siga siendo
`active` (este campo no se modifica al expirar — ver `design.md`).

#### Scenario: Credencial expirada por renovación se muestra vencida

- **WHEN** una credencial fue expirada (por una renovación confirmada) y su `validUntil` quedó en el pasado
- **AND** se consulta el listado de credenciales (UC02)
- **THEN** esa credencial se muestra con indicador "Vencida", no como "Activa", independientemente del valor de `credentialStatus` en su JSON

#### Scenario: Credencial vigente se sigue mostrando activa

- **WHEN** una credencial tiene `validUntil` en el futuro y `credentialStatus` en `active`
- **THEN** se sigue mostrando como "Activa" (sin cambios respecto al comportamiento actual)
