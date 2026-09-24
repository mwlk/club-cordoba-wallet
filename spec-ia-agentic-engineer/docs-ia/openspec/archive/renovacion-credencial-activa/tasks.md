# Tasks: renovacion-credencial-activa

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/AGENTS.md`, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Releer `CreateCredentialCommandHandler.cs`, `ICredentialRepository.cs`/`CredentialRepository.cs`, `IssuerService.cs` (formato de `validUntil` dentro de `vc_json`), `GetCredentialsQueryHandler.cs` (patrón de parseo del JSON).
- [x] 0.3 Identificar archivos afectados (ver `design.md` seccion Contrato tecnico).

## 1. Validacion de spec

- [x] 1.1 Confirmar que cada escenario en `specs/renovacion-credencial-activa/spec.md` sea testeable.
- [x] 1.2 Agregar escenarios faltantes antes de programar.

## 2. Implementacion

### Backend

- [x] 2.1 `ICredentialRepository`: agregar `GetActiveByMemberIdAsync(Guid memberId, CancellationToken ct)` (lee todas las del member, filtra por `validUntil` del JSON > `DateTime.UtcNow`, devuelve **`List<Credential>`**, vacía si no hay ninguna — puede haber más de una, ver Riesgo 2 de `design.md`) y `Update(Credential credential)` (sin `SaveChangesAsync`, mismo patrón que `AddAsync`).
- [x] 2.2 `Credential` (Domain): agregar método `ExpireNow()` (o similar) que reescriba `validUntil` dentro de `VcJson` al valor actual, manteniendo el resto del JSON intacto (usar `JsonNode`/`JsonDocument`, no reconstruir el JSON a mano). **No tocar `credentialStatus`** (queda `active`; ver `design.md` seccion "UI — vigencia visual en UC02" sobre por qué).
- [x] 2.3 Nueva query: `GetActiveCredentialByDniQuery` + handler en `Application/Features/Credentials/Queries/GetActiveCredentialByDni/`, usando `IMemberRepository.GetByDniAsync` + `ICredentialRepository.GetActiveByMemberIdAsync`. DTO de respuesta: `ActiveCredentialDto(Guid CredentialId, DateTime ValidUntil)` — si hay más de una activa, devolver la de `ValidUntil` más lejano.
- [x] 2.4 `CreateCredentialCommand`: agregar campo `ConfirmarRenovacion` (bool, default false).
- [x] 2.5 `CreateCredentialCommandHandler`: antes de invocar al Issuer, si el member (nuevo o existente) tiene alguna credencial activa:
  - si `ConfirmarRenovacion == false` -> `Result.Fail("ActiveCredentialExists")` **sin `data`** (el `Result<T>` de este handler es `Result<CreateCredentialResult>`; no hay forma de adjuntar un `ActiveCredentialDto` ahí sin tocar `Result.cs` — se decidió no hacerlo, ver `design.md` Decisiones), sin persistir nada.
  - si `ConfirmarRenovacion == true` -> marcar **todas** las activas encontradas para expirar (`ExpireNow()` + `credentialRepository.Update(...)` por cada una), seguir el flujo normal (Issuer, nueva `Credential`, `AddAsync`), un solo `unitOfWork.SaveChangesAsync` al final.
- [x] 2.6 `Errors.resx`: agregar clave `ActiveCredentialExists` (mensaje genérico, sin placeholder de fecha).
- [x] 2.7 `CredentialsController`: nuevo endpoint `GET /api/credentials/members/active-credential?dni=` -> invoca la nueva query, `Ok(ToResponse(result))`. Actualizar `CreateCredentialRequest`/`Create` para pasar `ConfirmarRenovacion` al command.

### Frontend

- [x] 2.8 `CredentialsService`: nuevo método `getActiveCredential(dni: string)` (GET al nuevo endpoint) y actualizar `create(...)` para aceptar `confirmarRenovacion?: boolean` en el payload.
- [x] 2.9 Dialog de confirmación: se reutilizó `ConfirmationDialogComponent` (`shared/components/confirmation-dialog/`, ya existía, sin uso previo) — no se creó componente nuevo, título + mensaje con la fecha real.
- [x] 2.10 `credential-create.component.ts#submit()`: el chequeo va por el valor **actual del control `dni` del form** (no `memberFound`) y, si tiene formato válido, consulta `getActiveCredential(dni)`. Si `data` no es null, abre el dialog con ese `validUntil`; solo continúa el POST si el usuario confirma, pasando `confirmarRenovacion: true`. Si cancela, no hace nada (no toca `submitting`).
- [x] 2.11 Caso borde del POST devolviendo `ActiveCredentialExists` (race condition) cubierto por el manejo de error ya existente (`error.interceptor.ts` + reset de `submitting`), sin código adicional.
- [x] 2.12 `credential-card.component.ts` (detalle UC02) **y** `credential-list.component.ts` (tile del listado UC02, archivo separado con su propio HTML de estado — detectado en implementación, no estaba en el plan original) — ambos con `isExpired`, ambos templates muestran "Vencida" quando `validUntil` pasó, sin importar `credentialStatus`.

## 3. Validacion

- [x] 3.1 4 tests nuevos en `CreateCredentialCommandHandlerTests` (sin activa, activa sin confirmar, activa confirmada, dos activas confirmadas) + 4 en `GetActiveCredentialByDniQueryHandlerTests` (sin member, sin creds, solo vencidas, activa) + 2 en `credential-card.component.spec.ts` (`isExpired` con validUntil futuro/pasado). Total backend: 27/27 unit + 5/5 integration. Frontend: 5/5.
- [x] 3.2 `dotnet test tests/ClubCordobaWallet.Tests.Unit` (27/27) y `dotnet test tests/ClubCordobaWallet.Tests.Integration` (5/5, incluye nuevo test de flujo completo de renovación) — ambos verdes. `dotnet build` completo sin warnings. `ng build` y `ng test` (frontend) sin errores.
- [x] 3.3 Evidencia manual: verificación en navegador real (docker db+api + `ng serve` local) — DNI tipeado sin seleccionar candidato dispara el popup igual, confirmar renueva, listado UC02 muestra 1 "Activa" + credenciales viejas "Vencida" con badge correcto pese a `credentialStatus` sin tocar. Ver capturas en la conversación (no se generó archivo en `reports/`, evidencia queda en el historial de la sesión).
- [x] 3.4 Ejecutado `/sdd-review` — veredicto **PASS WITH GAPS** (0 CRITICAL, 4 WARNING, 3 INFO). Gaps cerrados en esta misma pasada:
  - WARNING 1 (escenario "falla de firma en renovación confirmada" sin test): agregado `Handle_Should_Not_Expire_Old_When_Issuer_Fails_During_Confirmed_Renewal`.
  - WARNING 2 (selección de la activa más lejana sin test): agregado `Handle_Should_Return_Furthest_ValidUntil_When_Multiple_Active_Credentials`.
  - WARNING 3 (doble submit no bloqueado): `submitting = true` se mueve al inicio de `submit()` (antes del chequeo async y del popup), guard `|| this.submitting` agregado, y se envolvió el chequeo previo en `try/catch` (cierra también el INFO de la promesa sin manejar). Backend: 29/29 unit + 5/5 integration. Frontend: 5/5.
  - WARNING 4 (`reports/` vacío): cerrado con [reports/verificacion-2026-09-23.md](reports/verificacion-2026-09-23.md).
  - INFO 1 (duplicación de `isExpired`) e INFO 3 (`documentation.md` pendiente): quedan documentadas como deuda conocida en `documentation.md` sección 6, no bloqueante.

## 4. Documentacion Confluence / Notion

- [x] 4.1 Completar `documentation.md` con las 6 secciones de `spec-ia-agentic-engineer/docs-ia/openspec/_templates/documentation.md`.
- [x] 4.2 Documentar problema, funcionamiento, cambios, scripts/migraciones (ninguno), pruebas, impacto, riesgos (firma HMAC de la credencial vieja queda desactualizada, más los hallazgos del `/sdd-review`), rollback y mantenimiento.
- [x] 4.3 Confirmar que no hace falta ninguna migración de EF Core (mismo esquema) — confirmado, `credentials.vc_json` es el único dato tocado.
- [x] 4.4 Evidencia en `reports/`: [verificacion-2026-09-23.md](reports/verificacion-2026-09-23.md).
- [x] 4.5 `documentation.md` listo para copiar a Confluence/Notion — ver cierre abajo.

## 5. Archive

- [x] 5.1 Validacion (secciones 1-3) y documentacion (seccion 4) cerradas — `/sdd-review` PASS WITH GAPS con gaps resueltos, `documentation.md` en estado "Aplicado y verificado".
- [x] 5.2 Spec estable creada en `spec-ia-agentic-engineer/docs-ia/openspec/specs/renovacion-credencial-activa/spec.md` (misma spec de la delta, formato plano). Además, `specs/alta-credencial/spec.md` actualizada con una nota MODIFIED en el escenario "Alta exitosa de un socio existente (renovación)" — el comportamiento base de UC01 cambió (ahora exige confirmación si hay credencial activa).
- [x] 5.3 Change movido a `spec-ia-agentic-engineer/docs-ia/openspec/archive/renovacion-credencial-activa/`.
