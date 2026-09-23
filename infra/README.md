# Infra

Este proyecto no requiere un `init.sql` manual: el schema de PostgreSQL se gestiona con **EF Core Migrations**, ya incluidas en el repo y aplicadas automáticamente al arrancar la API (ver `backend/README.md`).

## Servicios (docker-compose.yml, en la raíz)

| Servicio | Imagen | Puerto host |
|---|---|---|
| `db` | `postgres:18-alpine` | 5432 |
| `api` | build de `./backend` | 5000 |
| `ui` | build de `./frontend` (nginx) | 4200 |

## Variables de entorno

Las 3 vars que lee `docker-compose.yml` tienen default dev-safe, así que `docker-compose up --build` funciona sin crear `.env`. Para pisarlas con valores propios, copiar `.env.example` a `.env` en la raíz del repo:

```
DB_PASSWORD=              # contraseña de PostgreSQL (default: devpass)
HMAC_SECRET_KEY=          # clave secreta del Issuer — sin esto la API no arranca (default: dev-secret-key-change-me)
CORS_ALLOWED_ORIGINS=     # origen permitido para el frontend (default: http://localhost:4200)
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
