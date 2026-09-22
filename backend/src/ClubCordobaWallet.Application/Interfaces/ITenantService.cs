namespace ClubCordobaWallet.Application.Interfaces;

// "Tenant" = club-futbol, según el enunciado (rol de negocio, separado del Issuer).
// Arma el credentialSubject con los datos de negocio.
public interface ITenantService
{
    CredentialSubject BuildSubject(Member member, MemberCategory categoria, string foto);
}
