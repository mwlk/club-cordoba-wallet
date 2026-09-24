# Verificación renovacion-credencial-activa — 2026-09-23

## Tests automatizados

- `dotnet test tests/ClubCordobaWallet.Tests.Unit` → 29/29 pass. Incluye:
  - `CreateCredentialCommandHandlerTests`: sin activa (comportamiento sin
    cambios), activa sin confirmar (`ActiveCredentialExists`, nada persiste),
    activa confirmada (expira vieja + crea nueva, un commit), dos activas
    confirmadas (expira ambas), y falla de firma durante renovación
    confirmada (nada se toca, la vieja sigue vigente — agregado en la
    pasada de `/sdd-review`).
  - `GetActiveCredentialByDniQueryHandlerTests`: sin member, sin
    credenciales, solo vencidas, una activa, y dos activas (devuelve la de
    `validUntil` más lejano — agregado en la pasada de `/sdd-review`).
- `dotnet test tests/ClubCordobaWallet.Tests.Integration` → 5/5 pass. Incluye
  `Renewal_Flow_Requires_Confirmation_And_Expires_Old_Credential`: alta →
  `GET active-credential` la encuentra → segunda alta sin confirmar → 400
  `ActiveCredentialExists` sin persistir → alta confirmada → 201, listado
  pasa de 1 a 2 credenciales.
- `dotnet build` (backend completo): 0 warnings, 0 errors.
- `ng build` (frontend): sin errores. `ng test` (Karma/Jasmine): 5/5 pass,
  incluye `credential-card.component.spec.ts` (`isExpired` con validUntil
  futuro/pasado).

## Verificación manual (docker-compose db+api, `ng serve` local en :4200)

1. Por `curl` contra el backend real: alta nueva (`dni=41555666`) → `GET
   active-credential` la encuentra (`validUntil` correcto) → segunda alta
   sin confirmar → `400 ActiveCredentialExists`, sin `data` → listado sigue
   en 1 credencial (no persistió nada) → tercera alta con
   `confirmarRenovacion:true` → `201`, listado pasa a 2 (vieja con
   `validUntil` = momento de la renovación, `status` sigue en 0).
2. En el navegador (`credentials/new`): tipeado el DNI completo de un socio
   con credencial activa **sin clickear ningún candidato de la búsqueda**
   (autocompletado nombre/apellido/foto a mano) → al enviar, apareció el
   popup "Renovar credencial" con la fecha real de vigencia — confirma que
   el chequeo va por el valor del campo DNI, no por `memberFound` (gap
   detectado en la revisión de spec, antes de implementar).
3. Confirmado el popup → credencial emitida, resultado normal.
4. Listado (`/credentials`) tras la renovación: la credencial nueva se
   muestra "Activa" (verde) y las viejas "Vencida" (gris), pese a que su
   `credentialStatus` almacenado sigue en `active` — confirma que
   `isExpired` (por fecha) manda sobre `credentialStatus` en la UI, en
   ambos lugares que muestran estado (`credential-list` y `credential-card`
   del detalle expandible).

## Resultado

Todos los escenarios de `specs/renovacion-credencial-activa/spec.md`
verificados por test automatizado, por `curl` contra el backend real, o
manualmente en navegador. `/sdd-review` corrido con veredicto **PASS WITH
GAPS** (4 WARNING, 3 INFO); los 3 gaps de mayor impacto (2 escenarios de
spec sin test + doble-submit no bloqueado) se cerraron en la misma sesión.
Quedan como deuda conocida y documentada (no bloqueante): este reporte en
`reports/` cierra el WARNING de evidencia faltante; la duplicación de
`isExpired` entre `credential-card` y `credential-list` queda anotada en
`documentation.md` sección 6 como mejora futura.
