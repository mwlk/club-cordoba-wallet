# Infra

Este proyecto no requiere un `init.sql` manual: el schema de PostgreSQL se gestiona con **EF Core Migrations**, ya incluidas en el repo y aplicadas automáticamente al arrancar la API (ver `backend/README.md`).

## Servicios (docker-compose.yml, en la raíz)

| Servicio | Imagen | Puerto host (default) |
|---|---|---|
| `db` | `postgres:18-alpine` | 5432 |
| `api` | build de `./backend` | 5000 |
| `ui` | build de `./frontend` (nginx) | 4200 |

Configurable vía `DB_PORT`/`API_PORT`/`UI_PORT` (ver variables abajo).

## Variables de entorno

Todas las vars que lee `docker-compose.yml` tienen default dev-safe, así que `docker-compose up --build` funciona sin crear `.env`. Para pisarlas con valores propios, copiar `.env.example` a `.env` en la raíz del repo:

```
DB_PASSWORD=              # contraseña de PostgreSQL (default: devpass)
HMAC_SECRET_KEY=          # clave secreta del Issuer — sin esto la API no arranca (default: dev-secret-key-change-me)
CORS_ALLOWED_ORIGINS=     # origen permitido para el frontend (default: http://localhost:4200)

DB_PORT=                  # puerto publicado en el host para Postgres (default: 5432)
API_PORT=                 # puerto publicado en el host para la API (default: 5000)
UI_PORT=                  # puerto publicado en el host para el frontend (default: 4200)
                           # si se cambia, actualizar también CORS_ALLOWED_ORIGINS a juego

API_INTERNAL_PORT=        # puerto donde escucha Kestrel dentro de la red de Docker (default: 8080)
UI_INTERNAL_PORT=         # puerto donde escucha nginx dentro de la red de Docker (default: 80)
                           # solo hace falta tocarlos si algo interno ya usa 8080/80
```

## Levantar todo

```bash
docker-compose up --build
```

Con valores propios:

```bash
cp .env.example .env
# editar .env
docker-compose up --build
```
