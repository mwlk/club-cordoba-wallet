# Delta Spec: alta-credencial

## ADDED Requirements

### Requirement: Alta de credencial de socio (UC01)

El sistema DEBE permitir a un administrador del tenant dar de alta una
credencial de socio completando nombre, apellido, DNI, categoría y foto, y
DEBE emitir y persistir la VC firmada por el Issuer.

#### Scenario: Alta exitosa de un socio nuevo

- **WHEN** el administrador envía el formulario de alta con un DNI que no
  existe todavía como socio, con datos válidos
- **AND** el Issuer firma correctamente la credencial
- **THEN** el sistema crea un `Member` nuevo (DID + `numeroSocio` secuencial
  de 6 dígitos), arma el `credentialSubject`, obtiene la VC firmada del
  Issuer y persiste tanto el `Member` como la `Credential`
- **AND** responde con el `numeroSocio` asignado y la vigencia
  (`validFrom`/`validUntil`)

#### Scenario: Alta exitosa de un socio existente (renovación)

- **WHEN** el administrador envía el formulario de alta con un DNI que ya
  tiene un `Member` asociado
- **THEN** el sistema reutiliza el `Did` y `numeroSocio` existentes (no
  regenera ninguno de los dos) y persiste una nueva `Credential` para ese
  mismo `Member`

#### Scenario: Falla la firma del Issuer con socio nuevo

- **WHEN** el administrador envía el formulario de alta con un DNI que no
  existe todavía como socio
- **AND** el Issuer falla al firmar (por ejemplo, `Issuer:HmacKey` no está
  configurado)
- **THEN** el sistema NO persiste ningún `Member` nuevo ni ninguna
  `Credential`
- **AND** responde con error (`IssuerSigningFailed`) sin crear registros
  huérfanos

#### Scenario: Falla la firma del Issuer con socio existente

- **WHEN** el administrador envía el formulario de alta con un DNI de un
  socio ya existente
- **AND** el Issuer falla al firmar
- **THEN** el sistema NO persiste ninguna `Credential` nueva (el `Member`
  existente no se modifica ni se duplica)
- **AND** responde con error (`IssuerSigningFailed`)

#### Scenario: Generación de campos automáticos

- **WHEN** se emite una credencial (con o sin `Member` nuevo)
- **THEN** el sistema genera automáticamente `id` de la VC (URI
  `https://credenciales.futbol.com.ar/{Guid}`), `credentialSubject.id`
  (`did:example:{Guid}`, solo si el `Member` es nuevo), `type`
  (`["VerifiableCredential","SocioCredential"]`), `issuer`
  (`did:example:futbol`), `validFrom` (UTC), `validUntil` (`validFrom` + 1
  año), `credentialStatus` (`0`/active por defecto) y `proof` (HMAC-SHA256
  sobre el JSON canónico, sin incluir `proof`, en Base64)
- **AND** ninguno de esos campos es editable desde el formulario

#### Scenario: Canonicalización exacta para la firma

- **WHEN** el Issuer arma el JSON sobre el que calcula el HMAC
- **THEN** el JSON es compacto (sin espacios/saltos de línea), con las
  claves de primer nivel y las de `credentialSubject` en orden alfabético
  ordinal, fechas en formato `yyyy-MM-ddTHH:mm:ssZ` sin milisegundos, y
  caracteres no ASCII (tildes, ñ) sin escapar a `\uXXXX`

## MODIFIED Requirements

### Requirement: Persistencia transaccional de Member y Credential

El sistema DEBE persistir el `Member` (si es nuevo) y la `Credential` como
una única operación atómica, condicionada al éxito de la firma del Issuer.

#### Scenario: Commit único después de la firma

- **WHEN** el Issuer confirma la firma exitosamente
- **THEN** el sistema ejecuta un único `SaveChangesAsync` (vía
  `IUnitOfWork`) que persiste el `Member` nuevo (si aplica) y la
  `Credential` juntos

#### Scenario: Ningún commit si falla la firma

- **WHEN** el Issuer lanza `IssuerSigningException`
- **THEN** el sistema NO ejecuta ningún `SaveChangesAsync` — ni el `Member`
  recién construido en memoria ni la `Credential` llegan a la base

## REMOVED Requirements

Ninguno.
