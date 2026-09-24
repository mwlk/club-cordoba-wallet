# Frontend — Club Córdoba Wallet UI

Angular 22 · Angular Material · NgModules con lazy loading

## Instalar dependencias

El proyecto usa **pnpm** (ver `pnpm-lock.yaml` y `packageManager` en `package.json`) — no usar `npm install`, ignora el lockfile y puede instalar versiones distintas.

```bash
corepack enable   # si no lo tenés
pnpm install
```

## Levantar en desarrollo

```bash
pnpm start
```

Queda en `http://localhost:4200`, apuntando a `http://localhost:5000/api` (ver `src/environments/environment.ts`). Requiere el backend corriendo.

## Levantar con Docker

Ver `docker-compose.yml` en la raíz del repo. El frontend se compila con la configuración `production` de Angular (`environment.prod.ts`, `apiUrl: '/api'` relativo) y se sirve con nginx, que hace de reverse proxy: reenvía `/api/*` al contenedor `api` por la red interna de Docker (ver `nginx.conf.template`). Por eso el frontend no necesita saber host ni puerto del backend — ver [docs/decisiones.md](../docs/decisiones.md#infraestructura--docker).

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
