# Documentacion: renovacion-credencial-activa

> **Publicar:** Confluence o Notion (Markdown). Completar despues de implementar y verificar.
> Cualquier integrante del equipo debe entender el cambio sin leer el diff.

## Como pegar en Notion

1. Abrir este archivo en el repo y copiar todo el Markdown.
2. En Notion: pagina nueva -> pegar. Notion convierte encabezados, listas y tablas.
3. Si la tabla no se ve bien: menu del bloque -> Turn into -> Table, o Import -> Markdown y elegir el archivo.
4. Los bloques ` ```sql `, ` ```json ` o ` ```text ` deben quedar como Code; si no, crear bloque Code y pegar el contenido.
5. No usar HTML (`<br>`, entidades `&lt;`); usar Markdown compatible con Confluence y Notion.

## Como pegar en Confluence

Pegar como Markdown si el editor lo soporta, o copiar seccion por seccion. Mantener la misma fuente que Notion.

| Campo | Valor |
| --- | --- |
| Feature | Renovación de credencial activa (una sola credencial vigente por socio) |
| Ticket / referencia | Mejora propuesta, fuera del alcance obligatorio de `init.txt` |
| Estado | Aplicado y verificado (2026-09-23) |
| Fecha | 2026-09-23 |
| Responsable | Mirko |

---

## 1. Que problema resuelve

- **Situacion anterior:** un socio puede acumular N credenciales "vigentes" (sin vencer) simultáneamente, porque cada alta con el mismo DNI crea una `Credential` nueva sin tocar las anteriores.
- **Necesidad:** garantizar, como valor agregado no pedido explícitamente, que un socio tenga como máximo una credencial vigente a la vez.
- **Resultado esperado tras el cambio:** al reemitir para un socio con credencial vigente, el sistema pide confirmación y, si se confirma, la vieja queda vencida desde ese momento y la nueva pasa a ser la única vigente.

---

## 2. Como deberia funcionar

- **Flujo principal:** el usuario busca/tipea el DNI en el alta; si ese socio tiene una credencial vigente, al intentar emitir aparece un popup con la fecha de vigencia actual pidiendo confirmar. Si confirma, se emite la nueva y la vieja queda vencida. Si cancela, no pasa nada.
- **Reglas de negocio:** "activa" = `validUntil` (dentro del JSON de la VC) posterior al momento actual. Solo puede haber una consulta/decisión por vez — la confirmación viaja en el mismo POST de alta (`confirmarRenovacion: true`), no es un endpoint separado de "confirmar".
- **Casos limite / errores esperados:** si el POST llega sin confirmar para un socio con credencial activa, el backend corta con `ActiveCredentialExists` sin persistir nada (protege contra clientes que no pasan por el popup). Si falla la firma del Issuer durante una renovación confirmada, no se persiste nada (ni la nueva credencial ni la expiración de la vieja).
- **Configuracion:** ninguna nueva variable de entorno ni flag.

---

## 3. Que se modifico

### Codigo / aplicacion

| Area | Archivos / componentes | Cambio |
| --- | --- | --- |
| API | `CredentialsController.cs` | Nuevo endpoint `GET /api/credentials/members/active-credential?dni=`; `POST /api/credentials` acepta `confirmarRenovacion` |
| Application | `CreateCredentialCommand(Handler)`, nueva query `GetActiveCredentialByDni` | Chequeo de credencial activa + expiración condicionada a confirmación |
| Infrastructure | `CredentialRepository.cs`, `ICredentialRepository.cs` | Nuevos métodos `GetActiveByMemberIdAsync`, `Update` |
| Domain | `Credential.cs` | Nuevo método `ExpireNow()` |
| Resources | `Errors.resx` | Nueva clave `ActiveCredentialExists` |
| UI | `credential-create.component.ts/.html`, `CredentialsService`, nuevo dialog de confirmación | Chequeo previo + popup de confirmación antes de emitir |
| UI | `credential-card.component.ts/.html` | Nuevo helper `isExpired` para mostrar "Vencida" en UC02 sin depender de `credentialStatus` |

### Base de datos / persistencia

| Tipo | Nombre | Descripcion |
| --- | --- | --- |
| Tablas / entidades leidas | `members`, `credentials` | Igual que hoy, sin nuevas tablas |
| Tablas / entidades escritas | `credentials.vc_json` | Update del campo `validUntil` dentro del JSON de una o mas filas existentes (todas las activas del socio) |
| SP / migracion / script | Ninguno | Mismo esquema, sin migración EF Core |
| Datos de referencia | — | — |

### Scripts de BD o migraciones

**Sin cambios de esquema ni scripts de BD.**

---

## 4. Como probarla

| # | Escenario | Pasos | Resultado esperado | Evidencia |
| --- | --- | --- | --- | --- |
| 1 | Socio sin credencial activa | Alta con DNI nuevo | Emite normal, sin popup | Test unitario + manual |
| 2 | Socio con credencial activa, no confirma | Alta con DNI existente vigente, cancelar popup | No se envía request, nada cambia | Manual |
| 3 | Socio con credencial activa, confirma | Alta con DNI existente vigente, confirmar popup | Vieja queda con `validUntil` pasado, nueva vigente; listado UC02 muestra la vieja con badge "Vencida" (por `isExpired`, no por `credentialStatus`) y la nueva "Activa" | Test integración + manual |
| 4 | POST directo sin confirmar (bypass del popup) | POST a `/api/credentials` con DNI de socio con credencial activa, sin `confirmarRenovacion` | 400 `ActiveCredentialExists` (sin `data`), nada persistido | Test integración |
| 5 | DNI tipeado sin seleccionar candidato | Tipear DNI completo de socio con credencial activa sin clickear la sugerencia, enviar | Popup aparece igual (chequeo por valor del campo, no por `memberFound`) | Manual |

- **Ambiente:** local (docker-compose para `db`+`api`, `ng serve` local para el front) y `dotnet test`/`ng test`.
- **Precondiciones:** al menos un socio con credencial ya emitida.
- **Datos de prueba:** DNI de socio existente con credencial vigente.
- **Comandos ejecutados:**
  - `dotnet test tests/ClubCordobaWallet.Tests.Unit` → 29/29 pass.
  - `dotnet test tests/ClubCordobaWallet.Tests.Integration` → 5/5 pass.
  - `dotnet build` (backend completo) → 0 warnings, 0 errors.
  - `ng build` y `ng test --watch=false --browsers=ChromeHeadless` (frontend) → build sin errores, 5/5 tests pass.

**Verificacion tecnica:**

```text
curl -s -X POST http://localhost:5000/api/credentials -d '{"nombre":"...","dni":"...","confirmarRenovacion":false, ...}'
  -> 400 ActiveCredentialExists si ya hay una vigente y no se confirma
curl -s "http://localhost:5000/api/credentials/members/active-credential?dni=..."
  -> {"success":true,"data":{"credentialId":"...","validUntil":"..."}} o data:null
```

- **Evidencia en** `reports/`: [verificacion-2026-09-23.md](reports/verificacion-2026-09-23.md) — detalle de los 29+5 tests, el flujo por `curl`, y la verificación manual en navegador (incluye el caso de DNI tipeado sin seleccionar candidato).

---

## 5. Que impacto tiene

- **Usuarios:** operador que da de alta credenciales — ahora recibe un aviso antes de duplicar una credencial vigente.
- **Operacion / soporte:** ninguno adicional.
- **Otros modulos o sistemas:** UC02 (listado) necesita un ajuste propio (`credential-card`) para mostrar "Vencida" — `credentialStatus` no se toca al expirar, así que sin ese ajuste la vieja seguiría viéndose "Activa" (gap detectado en revisión, ya incorporado a `design.md`/`tasks.md`).
- **Rendimiento / volumen:** despreciable — una consulta extra por alta, sin nuevos índices.
- **Seguridad / permisos:** ninguno — no hay auth en el proyecto (fuera de alcance).
- **Riesgos conocidos:**
  - Reescribir `validUntil` de una credencial ya firmada deja su firma HMAC "desactualizada" respecto al JSON final si alguien la verificara — no es un riesgo real en este proyecto porque no hay verificación de firma implementada, pero se documenta como limitación conocida y deliberada.
  - El error `ActiveCredentialExists` del POST no devuelve `validUntil` estructurado (limitación del `Result<T>` compartido, no se toca esa clase) — el front depende de haberlo consultado antes vía el GET; si un cliente llama al POST directo sin pasar por ese GET, ve un mensaje sin fecha.
  - `/sdd-review` (2026-09-23) dio **PASS WITH GAPS**. Cerrados en la misma sesión: 2 escenarios de spec sin test (falla de firma en renovación confirmada, selección de la activa más lejana) y doble-submit no bloqueado en el frontend. Quedan como deuda no bloqueante (ver sección 6): duplicación de `isExpired` entre `credential-card` y `credential-list`, y un `try/catch` genérico (sin distinguir tipos de error) en el chequeo previo del frontend.
- **Rollback / recuperacion:** revertir el código del change; no hay migración de esquema que revertir. Credenciales ya expiradas en la práctica quedan con `validUntil` correcto según la decisión de negocio tomada.

---

## 6. Como mantenerla en el futuro

- **Spec OpenSpec:** `spec-ia-agentic-engineer/docs-ia/openspec/specs/renovacion-credencial-activa/` (tras archivar) y `spec-ia-agentic-engineer/docs-ia/openspec/archive/renovacion-credencial-activa/`.
- **Config por ambiente:** ninguna.
- **Monitoreo:** ninguno nuevo (Serilog existente cubre el flujo).
- **Deuda / mejoras:**
  - Si en el futuro se implementa verificación de firma, esta feature necesitaría re-firmar la credencial expirada (o modelar `validUntil` fuera del payload firmado) para no invalidar su integridad criptográfica.
  - `isExpired` (fecha vs `credentialStatus`) está duplicado en `credential-card.component.ts` y `credential-list.component.ts` — candidato a extraerse a un pipe o util compartido si se agrega un tercer lugar que muestre estado de credencial.
- **Contacto / dominio:** Mirko.

---

## Referencias en el repo

- `proposal.md`
- `design.md`
- `specs/renovacion-credencial-activa/spec.md`
- `tasks.md`
- `reports/` si existe
