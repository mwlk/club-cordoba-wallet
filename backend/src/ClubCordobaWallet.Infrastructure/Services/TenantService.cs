namespace ClubCordobaWallet.Infrastructure.Services;

// Rol de negocio (Tenant = club-futbol), separado del Issuer, tal cual
// pide el enunciado en el objetivo (sección 1).
public class TenantService : ITenantService
{
    public CredentialSubject BuildSubject(Member member, MemberCategory categoria, string foto)
    {
        return new CredentialSubject
        {
            Id = member.Did,
            Nombre = member.FirstName,
            Apellido = member.LastName,
            Dni = member.Dni,
            NumeroSocio = member.MemberNumber,
            Categoria = categoria,
            Foto = foto
        };
    }
}
