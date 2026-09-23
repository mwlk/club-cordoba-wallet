# Infra

Este proyecto no requiere un `init.sql` manual: el schema de PostgreSQL se gestiona con **EF Core Migrations**, aplicadas automáticamente al arrancar la API (ver `backend/README.md` para generar la migration inicial).

## Servicios (docker-compose.yml, en la raíz)

| Servicio | Imagen | Puerto host |
|---|---|---|
| `db` | `postgres:18-alpine` | 5432 |
| `api` | build de `./backend` | 5000 |
| `ui` | build de `./frontend` (nginx) | 4200 |

## Variables de entorno

Copiar `.env.example` a `.env` en la raíz del repo y completar:

```
DB_PASSWORD=              # contraseña de PostgreSQL
HMAC_SECRET_KEY=          # clave secreta del Issuer — sin esto la API no arranca
CORS_ALLOWED_ORIGINS=     # origen permitido para el frontend (default: http://localhost:4200)
```

## Levantar todo

```bash
cp .env.example .env
# editar .env con valores propios
docker-compose up --build
```
