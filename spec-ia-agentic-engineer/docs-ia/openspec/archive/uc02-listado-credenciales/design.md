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
- En frontend, en vez de un segundo argumento `error` en el `subscribe`
  (duplicaría `detectChanges()` en dos branches separados), se usa
  `catchError` en el `pipe()` del observable: el stream resuelve a un valor
  seguro (lista vacía / respuesta `success:false`) y un único callback
  `next` cubre éxito y fallo. El snackbar global de `error.interceptor.ts`
  sigue mostrando el mensaje sin cambios — el interceptor corre antes en la
  cadena HTTP, `catchError` en el componente no lo pisa.

## Flujo propuesto

```text
GET /api/credentials
  -> CredentialsController.List -> GetCredentialsQueryHandler.Handle
  -> credentialRepository.GetAllAsync() -> parse vc_json -> CredentialListDto[]
  -> 200 OK [] (vacío) | [...] (con datos)

Angular: credential-list.component.ts#ngOnInit
  -> credentialsService.list().pipe(
       catchError(() => of([]))   // <- agregado: sale del estado de carga ante error
     ).subscribe(list => { credentials = list; loaded = true; })
  -> template: *ngIf="loaded && credentials.length === 0" -> <app-empty-state>
```

```text
GET /api/credentials/{id}
  -> CredentialsController.GetById -> GetCredentialByIdQueryHandler.Handle
  -> 200 OK { success:true, data:{...} } | 404 { success:false, message:"..." }

Angular: credential-detail.component.ts#ngOnInit
  -> credentialsService.getById(id).pipe(
       catchError(() => of({ success:false, message:'', data:null }))   // <- agregado: cubre el 404 real (HttpErrorResponse)
     ).subscribe(response => { if (response.success) credential = response.data; else notFound = true; })
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

- Unitarias: sin cambios necesarios en backend (ya cubierto). No se agregó test de componente Angular (`ng test`) para el `catchError` — cubierto en cambio por verificación manual end-to-end (ver `reports/verificacion-2026-09-23.md`), más directo para este caso al requerir simular caída real del backend.
- Integración: `CredentialsEndpointTests` ya cubre lista vacía y flujo completo alta→listado→detalle; no se agregan casos nuevos de backend.
- E2E / manual: apagar el backend, abrir `/credentials` y `/credentials/{id-inexistente}` en el frontend y confirmar que ambas pantallas salen del estado de carga y muestran algo (estado vacío / no encontrada) en vez de quedar en blanco. Ejecutado, ver `reports/verificacion-2026-09-23.md`.

## Riesgos

- Ninguno relevante — cambio acotado a manejo de errores en dos componentes, sin tocar lógica de negocio ni contratos.

## Rollback

Quitar el `.pipe(catchError(...))` de los dos `subscribe()` — cambio de 2 archivos, sin dependencias ni migración de datos.
