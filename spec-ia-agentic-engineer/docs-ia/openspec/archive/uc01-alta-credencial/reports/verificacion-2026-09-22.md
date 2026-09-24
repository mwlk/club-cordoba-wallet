# Evidencia de verificación — uc01-alta-credencial (2026-09-22)

## Build completo del backend

```text
$ dotnet build
ok dotnet build: 7 projects, 0 errors, 24 warnings (00:00:04.77)
```

Nota: el scaffold original no compilaba (bugs preexistentes, fuera del
alcance de esta spec, corregidos para poder verificar):
- `ClubCordobaWallet.Application.csproj` no referenciaba
  `Microsoft.Extensions.DependencyInjection.Abstractions` (usado en
  `DependencyInjection.cs`).
- `ClubCordobaWallet.Api.csproj` no referenciaba `Microsoft.AspNetCore.OpenApi`
  (usado por `AddOpenApi()`/`MapOpenApi()`).
- `Program.cs` no tenía `using Microsoft.EntityFrameworkCore;` (necesario
  para `db.Database.Migrate()`).

## Unit tests

```text
$ dotnet test tests/ClubCordobaWallet.Tests.Unit
Passed!  - Failed: 0, Passed: 12, Skipped: 0, Total: 12, Duration: 62 ms
```

Incluye los 4 tests nuevos de `CreateCredentialCommandHandlerTests`
(escenarios ADDED/MODIFIED de `specs/alta-credencial/spec.md`):

- `Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_New_Member`
- `Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_Existing_Member`
- `Handle_Should_Call_SaveChanges_Once_When_Issuer_Succeeds_With_New_Member`
- `Handle_Should_Reuse_Existing_Member_Did_And_Number_On_Success`

Más los 8 tests preexistentes de `IssuerServiceTests` (canonicalización,
sin cambios).

## Integration tests — corridos con éxito (con flakiness de entorno conocida)

Reintentado más tarde en la sesión (después de subir a Postgres 18 y de la
mejora de búsqueda por prefijo). Se encontró y arregló un **bug real de
test** — no de infraestructura: `Search_Member_Not_Found_Returns_200_With_Success_False`
esperaba `success:false` cuando no hay coincidencias, pero el contrato
cambió (`mejora-busqueda-socio`): la búsqueda ahora es por prefijo y
siempre responde `Result.Ok` con una lista (vacía si no hay candidatos) —
`success:false` ya no aplica a "no encontrado". Corregido y renombrado a
`Search_Member_Not_Found_Returns_200_With_Empty_List`, ahora valida
`success:true` + `data` como array vacío.

Con ese fix, 3 corridas consecutivas de
`dotnet test tests/ClubCordobaWallet.Tests.Integration`:

```text
run 1: Passed! - Failed: 0, Passed: 3, Skipped: 0, Total: 3
run 2: Failed! - Failed: 1, Passed: 2, Skipped: 0, Total: 3  (Connection reset by peer)
run 3: Passed! - Failed: 0, Passed: 3, Skipped: 0, Total: 3
```

El fallo intermitente del run 2 es la misma causa ya documentada
anteriormente en esta sesión: Testcontainers usa `exec` sobre el socket de
Podman rootless para el wait-strategy `pg_isready` de Postgres, y esa
llamada a veces corta la conexión (`Connection reset by peer`) — no es
determinístico, no depende de qué test corre, y no es un bug de la
aplicación (`docker run hello-world` funciona bien, es específico de la
API de exec que Testcontainers necesita). Confirmado otra vez que **no**
está relacionado a las tareas de esta sesión: 2 de 3 corridas dieron 3/3
verde limpio, incluyendo `Full_Flow_Create_Then_List_Then_Detail` (que
ejercita el flujo completo con `IUnitOfWork`) y el caso de búsqueda ya
corregido.

**Pendiente real remanente**: correr esta suite en una máquina con Docker
Engine real (no Podman) para confirmar que el flakiness desaparece del
todo — no se puede eliminar desde este entorno.

## Migration inicial + stack completo (`docker-compose up --build`)

Generada la migration obligatoria (`dotnet ef migrations add Initial`) y
levantado el stack completo (db + api + frontend). Se encontraron y
corrigieron 4 issues adicionales del scaffold para poder arrancar:

- `docker-compose.yml`: healthcheck de `db` usaba `pg_isready -U vcwallet`
  sin `-d`, no garantizaba que la base `clubcordobawallet` existiera →
  agregado `-d clubcordobawallet`.
- `frontend/package.json`: `typescript: ~5.6.0` no es compatible con
  `@angular/compiler-cli@22` — peer dependency real verificada
  (`npm info @angular/compiler-cli@22 peerDependencies`) es
  `typescript: >=6.0 <6.1`. Corregido a `~6.0.0`.
- `frontend/angular.json`: `pnpm i` agregó `cli.analytics: false` (opt-out
  de telemetría de Angular CLI, sin impacto funcional).
- `backend/.../Program.cs`: agregado redirect de `/` a `/scalar/v1` en
  Development, para no dejar la raíz en 404 al levantar la API.

Diff completo revisado línea por línea, sin cambios fuera de estos 4 puntos.

## Verificación manual (frontend)

Stack levantado con éxito (`docker-compose up --build`), confirmando que
migration + fixes de arriba resuelven el arranque completo.

Al probar el flujo real en navegador (browser embebido, `ng serve` local +
API + Postgres) se encontró un **bug crítico no relacionado al Unit of
Work**: la UI quedaba congelada tras cualquier respuesta HTTP.

- Alta de credencial: el backend respondía `201 Created` con
  `{success:true, data:{memberNumber,...}}` (confirmado en Network tab), pero
  el botón de submit quedaba en "Emitiendo…" para siempre y el panel de
  confirmación nunca aparecía.
- Listado: `GET /api/credentials` respondía `200 OK` con los datos, pero la
  pantalla quedaba con los skeletons de carga indefinidamente.
- Se confirmó, inspeccionando la instancia del componente con
  `window.ng.getComponent(...)` (Angular DevTools API), que **el estado
  interno SÍ se actualizaba correctamente** (`submitting:false`,
  `result:{memberNumber:"000003",...}`, `credentials:[...]`, `loaded:true`)
  — el problema no era el código de negocio, era que la vista nunca se
  volvía a renderizar.
- Causa raíz confirmada: `zone.js` está cargado (`typeof Zone !== 'undefined'`
  → true) pero **no parchea `XMLHttpRequest.prototype.send` ni `window.fetch`
  en runtime** (ambos siguen siendo `[native code]`). Incluso `NgZone.run()`
  no logra entrar a la zona `"angular"` (`Zone.current.name` queda en
  `"<root>"` dentro del callback), así que ni forzar `ApplicationRef.tick()`
  desde `ApiService` funcionó. Se verificó que un click/tecla posterior sin
  relación (evento DOM, sí parcheado) disparaba un tick que "flusheaba" el
  estado ya actualizado — por eso el síntoma era intermitente (la búsqueda de
  socio parecía funcionar porque el usuario seguía tipeando después).
- Se comprobó que `window.ng.applyChanges(componente)` (equivalente a
  `ChangeDetectorRef.detectChanges()` del propio componente) sí renderiza de
  forma confiable. **Fix aplicado**: `this.cdr.detectChanges()` explícito al
  final de cada `.subscribe()` en `credential-list`, `credential-create`,
  `credential-detail` y `member-search`. Ver `docs/decisiones.md` (sección
  Frontend/UX) para el detalle completo.
- Reprobado en navegador tras el fix: alta completa (form + submit) muestra
  el panel "¡Credencial emitida!" con número de socio y vigencia, y el
  listado renderiza las cards inmediatamente sin esperar otra interacción.

**Pendiente real remanente**: no se probó puntualmente el caso "firma falla
→ 400" (`credential-create.component.ts#submit()`, callback `error`
agregado en 2.7) — sí se confirmó indirectamente que `this.cdr.detectChanges()`
también corre en esa rama, así que el botón debería liberarse igual, pero
falta la prueba explícita forzando el fallo del Issuer.

## Verificación end-to-end en `docker-compose` (los 3 servicios, build real)

Repetido todo lo anterior contra el stack completo levantado con
`docker-compose up --build` (no `ng serve`/`dotnet run` locales): `db`
(`postgres:18-alpine`, healthy), `api` (imagen `.NET` en modo `Production`) y
`ui` (build de Angular servido por nginx, no dev server). Confirmado en
navegador contra `http://localhost:4200`:

- Estado vacío inicial correcto.
- Alta completa (DNI nuevo, form + submit) → panel "¡Credencial emitida!"
  con número de socio y vigencia, en un solo intento — el fix de
  `ChangeDetectorRef.detectChanges()` funciona igual en el bundle de
  producción minificado (no era un artefacto del dev-server de Vite).
- Toast "Credencial emitida correctamente" (interceptor) al confirmar el
  alta.
- Volver al listado → la card nueva aparece de inmediato, sin clicks extra.
- Detalle de la credencial → renderiza al toque.
- Búsqueda de socio por prefijo (`mejora-busqueda-socio`) → tipeando "402"
  aparece "Docker Compose · DNI 40255001 · Socio #000001" en la lista de
  candidatos.

Esto también cierra la duda de si el bug de zone.js era específico del
dev-server (Vite/`ng serve`): **no lo es** — se reproduce y se soluciona
igual en el build de producción real.

## Verificación manual — extensión 5a (firma falla → no se persiste nada)

Cierra la tarea 3.2 (bloqueada a nivel de integration test por el fail-fast
de boot) con una prueba manual end-to-end real, sin tocar el fail-fast:

- Se forzó temporalmente `IssuerService.Issue()` para tirar
  `IssuerSigningException` incondicionalmente (cambio de una línea, archivo
  sin diffs previos en git, revertido con `git checkout` apenas terminó la
  prueba — no queda rastro en el código).
- Rebuild + restart del contenedor `api` (`docker compose up --build -d api`).
- Alta con DNI nuevo (`40388222`, "Falla Firma") → `POST /api/credentials`
  responde `400` con `{"success":false,"message":"No se pudo firmar la
  credencial. Intente nuevamente.","data":null}`.
- Toast de error visible en la UI ("No se pudo firmar la credencial. Intente
  nuevamente.") y el botón de submit se libera (vuelve a "Emitir
  credencial", no queda en "Emitiendo…").
- `GET /api/credentials` después del intento fallido: sigue devolviendo sólo
  la credencial previa ("Docker Compose") — el `Member` con DNI `40388222`
  **no quedó persistido**, confirmando la extensión 5a end-to-end (UI +
  API + DB) con Postgres 18.
- Revertido el cambio temporal (`git checkout` sobre `IssuerService.cs`,
  diff limpio confirmado) y rebuild final del contenedor `api` con el código
  original — stack quedó sano y funcionando.

Con esto, el escenario "firma falla" queda cubierto tanto a nivel unitario
(3.1) como manual end-to-end (este apartado). Sigue sin poder automatizarse
como integration test por el fail-fast de boot (ver 3.2).
