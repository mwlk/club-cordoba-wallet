# Lite: mejora-busqueda-socio

> Versión liviana de proposal+design+spec. Califica como lite: es una mejora
> de UX sobre un endpoint ya existente, no toca el modelo de persistencia
> (`members`/`credentials`), revert simple (volver a búsqueda exacta), no
> crítico para UC01 (el alta funciona igual con o sin esta mejora — hoy ya
> permite tipear el DNI manualmente si no se encuentra socio).

## Por qué

Detectado durante la verificación manual de UC01 en navegador: la búsqueda de
socio por DNI (`GET /credentials/members/search?dni=`) exige el DNI completo
exacto (7-8 dígitos) y sólo devuelve 0 ó 1 resultado. El usuario pidió que, al
buscar, si hay coincidencias se puedan ver en una lista y seleccionar una —
mejora de UX no exigida por el enunciado (que sólo pide "buscar por DNI"),
pero valiosa porque el operador del club puede no recordar el DNI completo.

## Qué cambia

**Backend** (`backend/src/ClubCordobaWallet.Application/Credentials/Queries/SearchMemberByDni/`):
- `SearchMemberByDniQueryHandler`: en vez de `GetByDniAsync` (exacto), usa un
  nuevo método de repositorio que busca por **prefijo** de DNI
  (`EF.Functions.Like(m.Dni, dni + "%")` o `StartsWith`, traducido a SQL
  `LIKE`), devuelve `List<MemberSearchDto>` capado con `.Take(10)`.
- `MemberSearchDto`: se agrega el campo `Dni` (hoy solo tiene `FirstName`,
  `LastName`, `MemberNumber`) — necesario para que el frontend sepa qué DNI
  exacto corresponde al candidato elegido.
- `IMemberRepository`: nuevo método `SearchByDniPrefixAsync(string prefix, int take, CancellationToken ct)` junto al `GetByDniAsync` existente (no se
  reemplaza, `GetByDniAsync` lo sigue usando el handler de alta para buscar
  exacto al crear/reusar el socio).
- `CredentialsController.SearchMember`: mismo endpoint y ruta, ahora `data`
  es un array (puede ser vacío) en vez de un objeto único o `null`. Sigue
  devolviendo siempre `200` (Result pattern, sin 404).

**Frontend** (`frontend/src/app/features/credentials/components/member-search/`):
- `member-search.component.ts`: se reemplaza el botón "Buscar socio por DNI"
  por búsqueda reactiva: un `FormControl` con `valueChanges.pipe(debounceTime(300), distinctUntilChanged(), filter(dni => dni.length >= 3), switchMap(dni => credentialsService.searchMembers(dni)))`, expuesto como
  `candidates$` y consumido con `| async` en el template (evita depender del
  bug de `zone.js` documentado en `docs/decisiones.md`, ya que `async` pipe
  se suscribe/desuscribe con el propio ciclo de Angular).
- Template nuevo: lista de candidatos (nombre, apellido, DNI, número de
  socio) clickeables. Al elegir uno, se emite `memberFound` con el candidato
  completo (incluye `dni`), y `credential-create.component.ts#onMemberFound`
  ahora también hace `patchValue({ dni: member.dni })` (hoy sólo parchea
  `nombre`/`apellido`).
- `credentials.service.ts`: nuevo método `searchMembers(dniPrefix: string): Observable<ApiResponse<MemberSearchResult[]>>` (o se renombra el existente,
  a definir en `sdd-apply` mirando el resto de usos).

**Docs**: actualizar `docs/decisiones.md` (fila de "Alta de socio — no hay
endpoint separado") y `docs/arquitectura.md` (paso 1 del flujo UC01) para
explicar candidatos + selección en vez de resultado único.

## Qué no cambia

- El modelo de datos (`members`) — la búsqueda por prefijo es una query
  distinta, no requiere columnas ni índices nuevos (el DNI ya es `UNIQUE`,
  una búsqueda `LIKE 'prefijo%'` es aceptable para el volumen de este
  sistema; no se agrega un índice de texto especial por ser fuera de alcance
  de una prueba técnica).
- El flujo de alta en sí (`POST /credentials`) — sigue igual, sólo cambia
  cómo se llega a completar el campo `dni` del formulario.
- `GetByDniAsync` (búsqueda exacta) sigue existiendo y se usa igual en
  `CreateCredentialCommandHandler` para resolver/crear el socio.

## Contrato técnico

`GET /credentials/members/search?dni={prefijo}` — antes:
```json
{ "success": true, "message": "MemberFound", "data": { "firstName": "...", "lastName": "...", "memberNumber": "000001" } }
```
Después:
```json
{ "success": true, "message": "MemberFound", "data": [ { "dni": "40123456", "firstName": "...", "lastName": "...", "memberNumber": "000001" } ] }
```
(`data: []` cuando no hay coincidencias, en vez de `success:false`/`data:null`).

## Escenarios

- **WHEN** se tipean 3+ dígitos de un DNI existente **THEN** aparece una
  lista con los socios cuyo DNI empieza con esos dígitos (hasta 10).
- **WHEN** no hay coincidencias **THEN** la lista queda vacía, sin error, y
  el operador puede seguir cargando el alta manualmente.
- **WHEN** se selecciona un candidato de la lista **THEN** el formulario se
  completa con `dni`, `nombre` y `apellido` de ese candidato, y esos campos
  quedan deshabilitados (igual que hoy).
- **WHEN** se tipean menos de 3 dígitos **THEN** no se dispara ninguna
  petición (evita golpear el backend en cada tecla desde el primer dígito).

## Pruebas previstas

- Backend: unit tests nuevos de `SearchMemberByDniQueryHandler` — 0
  candidatos, 1 candidato, varios candidatos, prefijo vacío.
- Frontend: verificación manual en navegador (tipear DNI parcial, ver lista,
  seleccionar, confirmar que el form se completa con el candidato correcto).

## Riesgos y rollback

- Riesgo: cambiar `data` de objeto a array es un cambio de contrato — sólo
  lo consume este frontend (no hay otros clientes), riesgo bajo.
- Rollback: volver `MemberSearchDto`/handler/controller a la versión de
  objeto único y el frontend a búsqueda por botón — ambos lados quedan en
  commits/diffs acotados y reversibles independientemente.
