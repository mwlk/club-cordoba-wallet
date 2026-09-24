# Modelo de datos

## Motor: PostgreSQL 18

Elegido por ser gratuito/ampliamente soportado en .NET vía Npgsql, con soporte nativo de secuencias (`member_number_seq`) y JSONB para otros usos. La VC firmada en sí se persiste en `text`, no `jsonb` — ver más abajo.

## Diagrama entidad-relación

```mermaid
erDiagram
    MEMBERS {
        uuid id PK
        string did UK "did:example:{guid} — generado una vez"
        string member_number UK "secuencial, 6 dígitos zero-padded"
        string first_name
        string last_name
        string dni UK
        timestamptz created_at
    }
    CREDENTIALS {
        uuid id PK
        uuid member_id FK
        text vc_json "VC completa firmada, texto plano exacto salido del Issuer (id, type, issuer, credentialSubject, validFrom, validUntil, credentialStatus, proof)"
        timestamptz created_at
    }
    MEMBERS ||--o{ CREDENTIALS : "puede tener múltiples"
```

## Por qué `vc_json` y no columnas descompuestas

El `IssuerService` produce un documento JSON ya firmado — el `proofValue` es el resultado de un HMAC calculado sobre una serialización canónica exacta (orden de claves, encoding, formato de fechas). Si se descompusiera la VC en columnas y se reconstruyera el JSON al leerla, cualquier diferencia mínima de serialización invalidaría la correspondencia con la firma original. Persistir el JSON tal cual salió del Issuer garantiza que lo que se guarda es exactamente lo que se firmó.

Las columnas propias de `credentials` se limitan a metadatos de la tabla (`id`, `member_id`, `created_at`); todo el contenido de la VC vive en `vc_json`. El listado y el detalle se arman parseando ese JSON en la capa de aplicación.

Por el mismo motivo, `vc_json` es `text` y no `jsonb`: Postgres reordena las claves y quita espacios al guardar un `jsonb`, así que el string exacto que se firmó dejaba de ser recuperable byte a byte. Ver [decisiones.md](decisiones.md#modelo-de-datos) para el detalle del hallazgo.

## Por qué existe `members`

El enunciado no pide una entidad de socio explícita, pero especifica que `credentialSubject.id` (el DID) y `numeroSocio` deben "generarse una vez y persistirse" — lo cual describe un ciclo de vida propio, independiente del de cada credencial. Ver [decisiones.md](decisiones.md) para el detalle completo de esta decisión.

## Secuencia `member_number_seq`

```sql
CREATE SEQUENCE member_number_seq START 1 INCREMENT 1;
-- uso: SELECT nextval('member_number_seq') → formateado a "D6" → "000123"
```

Se acepta que puedan quedar gaps (números no usados) si el alta de un socio se confirma pero la emisión de su primera credencial falla luego — las secuencias de PostgreSQL no son transaccionales. No hay requisito de continuidad estricta en el enunciado.

## Migrations

El schema se gestiona con EF Core Migrations (no `init.sql` manual). La migration inicial ya está incluida en el repo y se aplican todas automáticamente al arrancar la API (`db.Database.Migrate()` en `Program.cs`) — ver [backend/README.md](../backend/README.md#migrations) para el detalle y para generar una nueva si se modifica el modelo.
