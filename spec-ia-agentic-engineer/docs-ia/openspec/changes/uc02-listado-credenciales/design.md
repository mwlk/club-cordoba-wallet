# Design: uc02-listado-credenciales

## Contexto

Backend ya correcto (verificado en el código):

- `GetCredentialsQueryHandler.Handle` → `credentialRepository.GetAllAsync(ct)`,
  parsea `vc_json` con `JsonDocument`, arma `CredentialListDto` con foto,
  nombre, apellido, categoría, `numeroSocio`, vigencia y estado. Si no hay
  filas, `credentials` es una lista vacía y el `Select(...).ToList()` también
  devuelve lista vacía — sin código especial para el estado vacío, es una
  consecuencia natural del LINQ. Cubierto por
  `CredentialsEndpointTests.Get_Credentials_When_Empty_Returns_Empty_List`.
- `GetCredentialByIdQueryHandler.Handle` → si no existe, `Result.Fail("CredentialNotFound")`
  → el controller responde `404`. Si existe, arma `CredentialDetailDto` con
  los campos mínimos más `issuer` y `proof.type` (sin `proofValue` crudo).

Frontend con gap: ambos componentes de UC02 (`credential-list`,
`credential-detail`) llaman `.subscribe(callback)` con un solo argumento
(next), sin segundo argumento (error). RxJS no propaga ese error a ningún
otro lado más que la consola/el interceptor global — el estado local del
componente (`loaded`, `notFound`) nunca se actualiza.

## Decisiones

- No tocar el backend: ya cumple el enunciado y ya tiene test de integración
  para el estado vacío. Cambiar código que ya funciona sin necesidad
  introduciría riesgo sin beneficio.
- En frontend, agregar el segundo argumento (`error`) a ambos `subscribe`,
  siguiendo el mismo patrón que ya usa el resto de la app para errores
  (el snackbar global de `error.interceptor.ts` ya muestra el mensaje; el
  componente solo necesita salir de su estado de "cargando" indefinido).

## Flujo propuesto

```text
GET /api/credentials
  -> CredentialsController.List -> GetCredentialsQueryHandler.Handle
  -> credentialRepository.GetAllAsync() -> parse vc_json -> CredentialListDto[]
  -> 200 OK [] (vacío) | [...] (con datos)

Angular: credential-list.component.ts#ngOnInit
  -> credentialsService.list().subscribe(
       list  => { credentials = list; loaded = true; },
       error => { loaded = true; }   // <- agregado: sale del estado de carga
     )
  -> template: *ngIf="loaded && credentials.length === 0" -> <app-empty-state>
```

```text
GET /api/credentials/{id}
  -> CredentialsController.GetById -> GetCredentialByIdQueryHandler.Handle
  -> 200 OK { success:true, data:{...} } | 404 { success:false, message:"..." }

Angular: credential-detail.component.ts#ngOnInit
  -> credentialsService.getById(id).subscribe(
       response => { if (response.success) credential = response.data; else notFound = true; },
       error    => { notFound = true; }   // <- agregado: cubre el 404 real (HttpErrorResponse)
     )
```

## Contrato técnico

- Endpoints: `GET /api/credentials`, `GET /api/credentials/{id}` — sin cambios de contrato.
- Request/Response: sin cambios.
- Errores: sin cambios de comportamiento en backend; frontend ahora refleja el error en el estado del componente.

## Persistencia

- Entidades leídas: `Credential` (con `Member` embebido en `vc_json`, no requiere join).
- Entidades modificadas: ninguna (solo lectura).
- Transacción: no aplica (queries de solo lectura).
- Concurrencia: no aplica.

## Pruebas

- Unitarias: sin cambios necesarios en backend (ya cubierto). Si se agrega test de componente Angular, usar Jasmine/Karma existente (`ng test`) para `credential-list.component` y `credential-detail.component` con un `CredentialsService` mockeado que devuelva error.
- Integración: `CredentialsEndpointTests` ya cubre lista vacía y flujo completo alta→listado→detalle; no se agregan casos nuevos de backend.
- E2E / manual: apagar el backend, abrir `/credentials` y `/credentials/{id-inexistente}` en el frontend y confirmar que ambas pantallas salen del estado de carga y muestran algo (estado vacío / no encontrada) en vez de quedar en blanco.

## Riesgos

- Ninguno relevante — cambio acotado a manejo de errores en dos componentes, sin tocar lógica de negocio ni contratos.

## Rollback

Quitar el segundo argumento de los dos `subscribe()` — cambio de 2 archivos, sin dependencias ni migración de datos.
