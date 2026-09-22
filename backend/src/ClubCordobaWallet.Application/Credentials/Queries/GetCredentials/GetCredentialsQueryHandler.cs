using System.Text.Json;
using ClubCordobaWallet.Application.Credentials.Dtos;

namespace ClubCordobaWallet.Application.Credentials.Queries.GetCredentials;

// UC02: si no hay credenciales, se devuelve lista vacía -> el frontend
// resuelve el estado vacío (extensión 2a del enunciado).
public class GetCredentialsQueryHandler(ICredentialRepository credentialRepository)
    : IQueryHandler<GetCredentialsQuery, List<CredentialListDto>>
{
    public async Task<List<CredentialListDto>> Handle(GetCredentialsQuery query, CancellationToken ct)
    {
        var credentials = await credentialRepository.GetAllAsync(ct);

        return credentials.Select(c =>
        {
            var vc = JsonDocument.Parse(c.VcJson).RootElement;
            var subject = vc.GetProperty("credentialSubject");
            return new CredentialListDto(
                c.Id,
                subject.GetProperty("foto").GetString()!,
                subject.GetProperty("nombre").GetString()!,
                subject.GetProperty("apellido").GetString()!,
                Enum.Parse<MemberCategory>(subject.GetProperty("categoria").GetString()!),
                subject.GetProperty("numeroSocio").GetString()!,
                vc.GetProperty("validFrom").GetDateTime(),
                vc.GetProperty("validUntil").GetDateTime(),
                (CredentialStatus)vc.GetProperty("credentialStatus").GetInt32()
            );
        }).ToList();
    }
}
