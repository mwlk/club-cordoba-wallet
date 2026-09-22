namespace ClubCordobaWallet.Domain.Entities;

// El enunciado no define una entidad "socio" explícita, pero sí exige que
// credentialSubject.id (el DID) y numeroSocio se "generen una vez y se
// persistan". Eso describe un ciclo de vida propio, independiente de cada
// credencial emitida -> de ahí esta entidad. Ver docs/decisiones.md (D1-D3).
public class Member
{
    public Guid Id { get; private set; }
    public string Did { get; private set; } = default!;
    public string MemberNumber { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Dni { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public ICollection<Credential> Credentials { get; private set; } = new List<Credential>();

    private Member() { }

    public static Member Create(string firstName, string lastName, string dni, string memberNumber)
    {
        return new Member
        {
            Id = Guid.NewGuid(),
            Did = $"did:example:{Guid.NewGuid()}",
            MemberNumber = memberNumber,
            FirstName = firstName,
            LastName = lastName,
            Dni = dni,
            CreatedAt = DateTime.UtcNow
        };
    }
}
