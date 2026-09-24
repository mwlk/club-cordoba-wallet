# Spec: listado-credenciales

### Requirement: Listado de credenciales emitidas (UC02)

El sistema DEBE permitir a un administrador del tenant ver las credenciales
emitidas, con foto, nombre, apellido, categoría, número de socio, vigencia y
estado.

#### Scenario: Listado con credenciales existentes

- **WHEN** el administrador abre la pantalla de listado
- **AND** hay una o más credenciales emitidas
- **THEN** el sistema recupera las credenciales del tenant y las muestra con
  foto, nombre, apellido, categoría, número de socio, vigencia y estado

#### Scenario: Estado vacío

- **WHEN** el administrador abre la pantalla de listado
- **AND** no hay ninguna credencial emitida
- **THEN** el sistema muestra un estado vacío en vez de una lista

#### Scenario: Detalle expandible

- **WHEN** el administrador selecciona una credencial del listado
- **THEN** el sistema muestra el detalle completo, incluyendo los campos
  técnicos del protocolo (`issuer`, `proof.type`) sin exponer `proofValue`
  crudo

#### Scenario: Credencial no encontrada

- **WHEN** el administrador solicita el detalle de una credencial con un id
  que no existe
- **THEN** el sistema responde con un error de "no encontrada" y la pantalla
  de detalle lo refleja (no queda en blanco)

### Requirement: Manejo de error de red en las pantallas de listado y detalle

El sistema DEBE reflejar en el estado de la pantalla cualquier error de red
al listar o consultar credenciales, no solo mostrarlo en un mensaje
transitorio (snackbar).

#### Scenario: Error de red al listar

- **WHEN** el `GET /api/credentials` falla por un error de red o del
  servidor
- **THEN** la pantalla de listado sale del estado de "cargando" (no queda en
  blanco indefinidamente), aunque no tenga datos para mostrar

#### Scenario: Error de red al ver el detalle

- **WHEN** el `GET /api/credentials/{id}` falla (404 u otro error)
- **THEN** la pantalla de detalle marca el estado de "no encontrada" en vez
  de quedar en blanco
