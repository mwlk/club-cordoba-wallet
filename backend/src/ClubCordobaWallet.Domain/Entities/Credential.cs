using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClubCordobaWallet.Domain.Entities;

// Persiste la VC completa (id, type, issuer, credentialSubject, validFrom,
// validUntil, credentialStatus, proof) tal cual salió firmada del Issuer,
// como JSON. Ver docs/decisiones.md sobre por qué JSONB y no columnas.
public class Credential
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public string VcJson { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public Member Member { get; private set; } = default!;

    private Credential() { }

    public static Credential Create(Guid memberId, string vcJson)
    {
        return new Credential
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            VcJson = vcJson,
            CreatedAt = DateTime.UtcNow
        };
    }

    // renovacion-credencial-activa: al renovar, la credencial vieja no se
    // re-firma (no hay verificación de firma implementada, fuera de
    // alcance) — solo se reescribe validUntil dentro de su vc_json ya
    // persistido, para que deje de contar como "activa". credentialStatus
    // NO se toca acá (ver docs-ia/openspec/.../design.md, "UI — vigencia
    // visual en UC02": ese campo es para revocación real, distinto concepto).
    public void ExpireNow()
    {
        var node = JsonNode.Parse(VcJson)!.AsObject();
        node["validUntil"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        VcJson = node.ToJsonString(new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
