# Frontend — Club Córdoba Wallet UI

Angular 22 · Angular Material · NgModules con lazy loading

## Instalar dependencias

```bash
npm install
```

## Levantar en desarrollo

```bash
npm start
```

Queda en `http://localhost:4200`, apuntando a `http://localhost:5000/api` (ver `src/environments/environment.ts`). Requiere el backend corriendo.

## Levantar con Docker

Ver `docker-compose.yml` en la raíz del repo — el frontend se sirve con nginx sobre el build de producción (configuración `docker`, ver `src/environments/environment.docker.ts`).

## Estructura

```
src/app/
├── core/                    # Servicios HTTP, interceptors, guards, modelos/DTOs
├── shared/                  # SharedModule: componentes y pipes reutilizables
│   └── components/
│       └── credential-card/ # Carnet visual — usado en detalle y confirmación post-alta
└── features/
    └── credentials/         # Módulo con lazy loading: listado, alta, detalle
```

## Pantallas

- **Listado** (`/credentials`): cards con foto, nombre, categoría, número de socio, vigencia y estado. Estado vacío si no hay credenciales.
- **Alta** (`/credentials/new`): buscador de socio por DNI (autocompleta si existe) + formulario (nombre, apellido, categoría, foto). Al confirmar, muestra número de socio y vigencia antes de volver al listado.
- **Detalle** (`/credentials/:id`): carnet visual + sección "detalles de seguridad" colapsable (sin JSON crudo ni términos técnicos sin traducir).

Ver [docs/decisiones.md](../docs/decisiones.md) para las decisiones de UX tomadas.
