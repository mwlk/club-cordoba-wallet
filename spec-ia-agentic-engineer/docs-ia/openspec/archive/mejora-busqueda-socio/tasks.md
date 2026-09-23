# Tasks: mejora-busqueda-socio

## 0. Revision de contexto

- [x] 0.1 Leer `README.md` del proyecto, `spec-ia-agentic-engineer/docs-ia/PROJECT_CONTEXT.md` y `spec-ia-agentic-engineer/docs-ia/openspec/config.yaml`.
- [x] 0.2 Inspeccionar patrones existentes: `SearchMemberByDniQueryHandler`, `MemberRepository`, `member-search.component.ts`, `credentials.service.ts`.
- [x] 0.3 Identificar archivos afectados (ver `lite.md`).

## 1. Backend

- [x] 1.1 Agregar `SearchByDniPrefixAsync(string prefix, int take, CancellationToken ct)` a `IMemberRepository`/`MemberRepository` (`Where(m => EF.Functions.Like(m.Dni, prefix + "%")).Take(take)`).
- [x] 1.2 Agregar `Dni` a `MemberSearchDto`.
- [x] 1.3 Actualizar `SearchMemberByDniQueryHandler` para devolver `Result<List<MemberSearchDto>>` usando el nuevo método de búsqueda por prefijo (capado a 10, siempre `Ok` — nueva key `MembersSearched`, se eliminó `MemberFound`/`MemberNotFound` de los `.resx` por quedar sin uso).
- [x] 1.4 Actualizar `CredentialsController.SearchMember` (tipo del handler inyectado + comentario) y `DependencyInjection.cs` (registro del handler con el nuevo tipo genérico).
- [x] 1.5 Mantener `GetByDniAsync` intacto (lo sigue usando `CreateCredentialCommandHandler`).

## 2. Frontend

- [x] 2.1 `credentials.service.ts`: método `searchMembers(dniPrefix: string)` devolviendo `Observable<ApiResponse<MemberSearchResult[]>>` (reemplaza `searchMemberByDni`).
- [x] 2.2 `member-search.component.ts`: reemplazado el botón por `FormControl` + `valueChanges` reactivo (`debounceTime(300)`, `distinctUntilChanged()`, filtro por longitud mínima 3, `switchMap`), expone `candidates$`.
- [x] 2.3 Template de `member-search`: lista de candidatos clickeables (nombre, apellido, DNI, número de socio) con `| async`. **Nota real** (no prevista en el diseño): el `async` pipe por sí solo no alcanzó — mismo bug de zone.js de `docs/decisiones.md` afecta también esta suscripción; se agregó `this.cdr.detectChanges()` en el `tap` del pipe, igual que en los demás componentes.
- [x] 2.4 `credential-create.component.ts#onMemberFound`: agregado `dni: member.dni` al `patchValue`.
- [x] 2.5 Mantenido estilo/arquitectura del proyecto (NgModules, Angular Material, Result pattern). Extra no previsto: se agregó `justSelected` para ocultar la lista tras elegir un candidato (si no, quedaba visible tapando el form).

## 3. Documentación

- [x] 3.1 Actualizado `docs/decisiones.md` (sección "Alta de socio — no hay endpoint separado") y `docs/arquitectura.md` (paso 1 del flujo UC01).

## 4. Validación

- [x] 4.1 Unit tests de `SearchMemberByDniQueryHandler`: 0, 1 y varios candidatos (`SearchMemberByDniQueryHandlerTests.cs`, 3 tests nuevos, 15/15 en verde en la suite completa).
- [x] 4.2 Verificación manual en navegador: tipear "401" muestra 3 candidatos con DNI/nombre/socio; seleccionar uno completa `dni`/`nombre`/`apellido` en el form y oculta la lista. Confirmado con `window.ng.getComponent(...)`.
- [x] 4.3 Escenarios de `lite.md` cubiertos (candidatos, lista vacía, selección, mínimo de caracteres).
- [x] 4.4 (no previsto) `CredentialsEndpointTests.Search_Member_Not_Found_Returns_200_With_Success_False` quedó desactualizado con el cambio de contrato (esperaba `success:false`, ahora es siempre `success:true` + lista vacía) — corregido y renombrado a `Search_Member_Not_Found_Returns_200_With_Empty_List`. Detalle en `uc01-alta-credencial/reports/verificacion-2026-09-22.md`.

## 5. Cierre

- [x] 5.1 Archivado con `/sdd-archive`.
