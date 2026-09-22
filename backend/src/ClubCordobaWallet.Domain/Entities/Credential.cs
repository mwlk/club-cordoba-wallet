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
}
