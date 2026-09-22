namespace ClubCordobaWallet.Application.Interfaces;

// Servicio in-process, sin endpoint propio (así lo define el enunciado 4.2.1).
// Recibe el credentialSubject + status y devuelve la VC completa YA FIRMADA
// como JSON canónico. Si algo falla (típicamente: falta la clave HMAC),
// lanza IssuerSigningException y el caller no debe persistir nada.
public interface IIssuerService
{
    Task<IssuedCredential> Issue(CredentialSubject subject, CredentialStatus status, CancellationToken ct);
}

public record CredentialSubject
{
    public string Id { get; init; } = default!;
    public string Nombre { get; init; } = default!;
    public string Apellido { get; init; } = default!;
    public string Dni { get; init; } = default!;
    public string NumeroSocio { get; init; } = default!;
    public MemberCategory Categoria { get; init; }
    public string Foto { get; init; } = default!;
}

public record IssuedCredential(string VcJson, string MemberNumberEcho, DateTime ValidFrom, DateTime ValidUntil);
