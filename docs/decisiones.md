# Decisiones de diseño — Club Córdoba Wallet

Este documento registra las decisiones tomadas frente a puntos no especificados (o ambiguos) en el enunciado de la prueba técnica, según pide la sección 2.1 ("Documentación de decisiones tomadas").

## Arquitectura

| Decisión | Elección | Justificación |
|---|---|---|
| Controller único | `CredentialsController`, con 4 endpoints (incluyendo búsqueda de socio) | Requisito explícito del enunciado (4.2.1): "un único controller (...) que recibe el alta, arma el credentialSubject, invoca al Issuer, persiste la credencial y expone el listado". El buscador de socio es una acción más del mismo controller, no uno nuevo. |
| Capas | Clean layered: `Domain` / `Application` / `Infrastructure` / `Api` | Opcional según el enunciado (4.2.1), se adoptó para separar responsabilidades y facilitar testing por capa. |
| CQRS | Manual (interfaces `ICommandHandler<T,R>` / `IQueryHandler<T,R>`), sin MediatR | Menos dependencias y "magia" para el alcance de la prueba; el patrón queda igual de explícito. |
| Result pattern | `Result<T>` con `MessageKey`/`ErrorKey` en vez de excepciones para casos esperados | Los mensajes se resuelven vía `.resx` (`Messages.resx` / `Errors.resx`), habilitando i18n futura sin tocar lógica de negocio. |
| Errores no controlados | Middleware global (`ExceptionHandlingMiddleware`) | Los controllers no llevan try/catch; una única política de manejo de errores. |
| Logging | Serilog, sink a consola | Structured logging, legible en `docker logs`. |
| Documentación de API / UI interactiva | `Microsoft.AspNetCore.OpenApi` (generación del documento OpenAPI) + **Scalar** (`Scalar.AspNetCore`) para la UI interactiva, en vez de Swashbuckle/Swagger UI | Desde .NET 9, la plantilla oficial de ASP.NET Core Web API reemplazó Swashbuckle por `Microsoft.AspNetCore.OpenApi` como generador nativo del documento OpenAPI (sin UI propia). Microsoft recomienda Scalar como cliente de UI interactiva sobre ese documento (`app.MapScalarApiReference()`), en vez de agregar Swashbuckle solo para tener una UI. Se sigue esa recomendación por ser el camino oficial actual del framework, sin dependencias de terceros no mantenidas por Microsoft para la generación del documento. Solo habilitado en `Development` (mismo criterio que ya tenía `MapOpenApi()`). Disponible en `http://localhost:5000/scalar/v1`. |

## Alta de socio — no hay endpoint separado

El enunciado no define un caso de uso de "alta de socio", pero exige que `credentialSubject.id` (el DID) y `numeroSocio` se **"generen una vez y se persistan"**. Eso implica una entidad con ciclo de vida propio, independiente de cada credencial emitida.

Se decidió:
- El formulario de alta expone únicamente los 5 campos que define la sección 4.1.3 (nombre, apellido, DNI, categoría, foto) — no se agregó ningún campo ni pantalla de "alta de socio" explícita.
- Internamente, el handler de `POST /credentials` busca un socio existente por DNI; si no existe, lo crea de forma transparente (nuevo DID + siguiente número de secuencia).
- Si el mismo DNI ya tiene socio, se **reutiliza** su DID y `numeroSocio` — se permiten múltiples credenciales por socio (ej. renovaciones), sin invalidar las anteriores.
- Se agregó un endpoint de búsqueda (`GET /credentials/members/search?dni=`) como mejora de UX para autocompletar el formulario — no rompe la restricción de controller único porque es una acción más del mismo controller, no una entidad de negocio nueva expuesta.
- (`mejora-busqueda-socio`) La búsqueda es por **prefijo** de DNI (no exacto) y devuelve una **lista** de hasta 10 candidatos (`dni`, `firstName`, `lastName`, `memberNumber`), no un objeto único — el operador puede no recordar el DNI completo. El formulario reactivo (`debounceTime`+`switchMap`) muestra la lista y el usuario elige un candidato, que completa `dni`/`nombre`/`apellido` y deshabilita estos dos últimos. `GetByDniAsync` (exacto) se mantiene intacto para resolver/crear el socio en el `POST /credentials`.

## Modelo de datos

| Decisión | Elección | Justificación |
|---|---|---|
| Persistencia de la VC | Un solo campo `vc_json` (JSONB), no columnas descompuestas | El JSON que sale del Issuer YA está firmado; reconstruirlo desde columnas arriesga no reproducir bit a bit el documento firmado. Se persiste tal cual. |
| Entidad `members` | Tabla separada de `credentials` | Necesaria para cumplir "se genera una vez y se persiste" sin recurrir a lookups sobre datos desnormalizados de `credentials`. |
| `numeroSocio` | Secuencia de PostgreSQL (`member_number_seq`), formateada a 6 dígitos zero-padded | Persistente entre reinicios (requisito del enunciado), formato acorde al ejemplo (`"000123"`). |
| Gaps en la secuencia | Aceptados | Las secuencias de PostgreSQL no son transaccionales; si el alta de socio se confirma pero luego falla la firma, ese número no se reutiliza. No hay requisito de continuidad estricta. |
| Motor | PostgreSQL 18 | Soporte nativo de JSONB, gratuito, ampliamente usado en .NET vía Npgsql. |
| Migrations | EF Core Migrations, aplicadas automáticamente al arrancar la API | El schema queda versionado junto al código; no requiere un `init.sql` separado. |

## Canonicalización y firma (sección 4.1.2)

| Decisión | Elección | Justificación |
|---|---|---|
| Orden de claves | `SortedDictionary`/`JsonObject` armado explícitamente en orden alfabético ordinal (`StringComparer.Ordinal`) | Exigido por el enunciado; se verificó que el orden dado coincide con orden ordinal estándar. |
| Encoding | `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` | `System.Text.Json` escapa unicode por defecto (`ñ` → `\u00F1`), lo que rompe la reproducibilidad de la firma. Advertido explícitamente en el enunciado. |
| Formato de fecha | `yyyy-MM-ddTHH:mm:ssZ`, truncado a segundos | Un único `DateTime.UtcNow` truncado, reutilizado para `validFrom` y `proof.created` (deben coincidir, según el enunciado). |
| `proofValue` | Base64 | El enunciado especifica textualmente "codificada en base64"; el ejemplo ilustrativo del documento parece hexadecimal, se prioriza el texto de la spec sobre el ejemplo. |
| Clave HMAC | Variable de entorno / `dotnet user-secrets` en desarrollo, nunca hardcodeada | Requisito explícito del enunciado. La app falla al arrancar (`fail fast`) si la clave no está configurada. |
| Falla de firma | No se persiste nada (ni `Member` si ya existía, ni `Credential`) | Requisito explícito (extensión 5a de UC01). Implementado con `IUnitOfWork`: `MemberRepository`/`CredentialRepository` solo hacen `Add`, el commit único ocurre en el handler tras el éxito del Issuer. |

## Enums

| Decisión | Elección | Justificación |
|---|---|---|
| `MemberCategory` | `enum { adulto, juvenil, niño }`, serializado con `JsonStringEnumConverter` | Los nombres del enum son directamente los valores que exige el `credentialSubject` — sin mapeos manuales que puedan introducir errores de serialización. |
| `CredentialStatus` | `enum { active = 0, revoked = 1, suspended = 2 }` | Serializa como integer (no string), tal cual el enunciado (`"credentialStatus":0`). |

## Frontend / UX

| Decisión | Elección | Justificación |
|---|---|---|
| Detalle de credencial | Vista tipo carnet + sección "detalles de seguridad" colapsable, sin JSON crudo | Pensado desde la UX real del administrador del club, que no tiene conocimiento técnico de DIDs, HMAC o VCs. Los campos técnicos del protocolo (opcionales según el enunciado) se muestran con lenguaje humano ("firma digital válida"), no como datos crudos. |
| Foto | Campo requerido, sin fallback automático si se deja vacío | El enunciado define `foto` como "Input usuario" — el sistema no debe generar el dato por su cuenta. |
| Arquitectura Angular | NgModules + lazy loading (no standalone components) | Decisión explícita del desarrollador para reforzar la separación de responsabilidades por feature. |
| Comunicación HTTP | `ApiService` base, consumido por `CredentialsService` | Un solo punto de configuración de base URL e interceptors; los feature services no conocen `HttpClient` directamente. |
| Detección de cambios tras HTTP (`cdr.detectChanges()` manual) | En `credential-list`, `credential-create`, `credential-detail` y `member-search`, cada `.subscribe()` a un método de `CredentialsService` termina con `this.cdr.detectChanges()` | Bug de entorno verificado en navegador real: `zone.js` queda cargado (`Zone` global existe) pero no parchea `XMLHttpRequest.prototype.send`/`fetch` en runtime (siguen siendo código nativo), y ni siquiera `NgZone.run()` logra entrar a la zona `"angular"` (`Zone.current.name` queda en `"<root>"`). Sin ese parche, Angular nunca se entera de que una petición HTTP terminó y la vista queda congelada (el estado del componente sí se actualiza, confirmado inspeccionando la instancia con `window.ng.getComponent(...)`) hasta el próximo evento DOM no relacionado (click, tecla) que sí dispara un tick — por eso el síntoma era "a veces funciona". Se comprobó con `window.ng.applyChanges(componente)` (equivalente a `ChangeDetectorRef.detectChanges()`) que forzar el CD del componente puntual sí funciona de forma confiable, mientras que `ApplicationRef.tick()`/`ngZone.run()` desde el `ApiService` centralizado no alcanzan. Por eso el fix quedó a nivel de componente y no en `ApiService`. Pendiente investigar la causa raíz exacta de por qué `zone.js` no parchea en este entorno (sospecha: caché de prebundling de Vite/dev-server desactualizada tras el bump de `typescript`) en una máquina limpia. |
| (`toast-exito-http`) Toast de éxito genérico en interceptor | Nuevo `success.interceptor.ts`, simétrico a `error.interceptor.ts`: cualquier `POST`/`PUT`/`DELETE` con 2xx y `{success:true, message}` dispara un snackbar con ese `message` real del backend | Antes sólo había feedback de error (interceptor) y de éxito ad-hoc (snackbar manual en `credential-create.component.ts#backToList()`). Centralizarlo evita repetirlo por feature y reutiliza el mensaje ya resuelto vía el `Result` pattern, sin inventar un texto genérico. Se excluye `GET` (listar/consultar no amerita toast) y cualquier 2xx con `success:false`. |
| Bug: texto invisible en inputs deshabilitados en modo oscuro | `frontend/src/styles.scss`, bloque `.mat-mdc-form-field`: se agregan `--mat-form-field-outlined-disabled-input-text-color`, `--mat-form-field-outlined-disabled-label-text-color` y `--mat-form-field-outlined-disabled-outline-color`, mapeados a `--cc-text-disabled`/`--cc-border` | Causa raíz (verificada inspeccionando el CSS generado, no supuesta): el color de texto disabled de Angular Material 22 (M3) no sale del token que veníamos seteando (`--mdc-outlined-text-field-input-text-color`, que sólo cubre el estado habilitado) sino de `color-mix(in srgb, var(--mat-sys-on-surface) 38%, transparent)`. Nuestro theme define `--sys-on-surface` (sin el prefijo `mat-`) para los componentes que sí leemos manualmente, pero nunca redefine `--mat-sys-on-surface` — la variable real que el M3 prebuilt theme usa internamente — así que en modo oscuro ese cálculo seguía devolviendo el valor fijo del theme claro (casi negro) al 38% de opacidad, invisible sobre fondo oscuro. Fix acotado: pisar los tres tokens de `disabled` de `mat-form-field` directamente, sin tocar `--mat-sys-*` global (evita efectos secundarios en otros componentes M3 que si dependen de ese token). Verificado en navegador real, ambos temas, con `credential-create` (campos `nombre`/`apellido` deshabilitados tras autocompletar por DNI). |

## Testing

| Decisión | Elección | Justificación |
|---|---|---|
| Alcance | Unit tests (servicios, handlers) + Integration tests (endpoints con Testcontainers) | El enunciado no exige tests explícitamente, pero la "calidad de código" se evalúa; el `IssuerService` es el componente más crítico y el que más fácil se rompe silenciosamente (encoding, orden de claves, formato de fechas). |
| DB para integration tests | PostgreSQL real vía Testcontainers | Evita falsos positivos de un motor in-memory que no reproduce el comportamiento real de JSONB/secuencias. |

## Fuera de alcance (explícito en el enunciado, sección 2.2)

No se implementó: verificación de credenciales, revocación o cambio de estado, autenticación/autorización, fidelidad estricta a la spec W3C VC/DID, paginación del listado.
