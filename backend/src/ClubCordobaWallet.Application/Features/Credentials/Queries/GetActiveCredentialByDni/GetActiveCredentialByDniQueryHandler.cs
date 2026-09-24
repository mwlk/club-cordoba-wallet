using System.Text.Json;
using ClubCordobaWallet.Application.Features.Credentials.Dtos;

namespace ClubCordobaWallet.Application.Features.Credentials.Queries.GetActiveCredentialByDni;

// renovacion-credencial-activa: consulta usada por el frontend antes de
// enviar el alta, para mostrar el popup de confirmación con la fecha real.
// Sin member o sin credencial vigente no es un error -> siempre Result.Ok,
// mismo criterio que SearchMemberByDniQueryHandler.
public class GetActiveCredentialByDniQueryHandler(
    IMemberRepository memberRepository,
    ICredentialRepository credentialRepository
) : IQueryHandler<GetActiveCredentialByDniQuery, Result<ActiveCredentialDto?>>
{
    public async Task<Result<ActiveCredentialDto?>> Handle(GetActiveCredentialByDniQuery query, CancellationToken ct)
    {
        var member = await memberRepository.GetByDniAsync(query.Dni, ct);
        if (member is null)
            return Result<ActiveCredentialDto?>.Ok(null);

        var active = await credentialRepository.GetActiveByMemberIdAsync(member.Id, ct);
        if (active.Count == 0)
            return Result<ActiveCredentialDto?>.Ok(null);

        // Si hubiera más de una activa (condición de carrera, ver design.md),
        // se informa la de vigencia más lejana.
        var latest = active
            .Select(c => new
            {
                c.Id,
                ValidUntil = JsonDocument.Parse(c.VcJson).RootElement.GetProperty("validUntil").GetDateTime()
            })
            .OrderByDescending(c => c.ValidUntil)
            .First();

        return Result<ActiveCredentialDto?>.Ok(new ActiveCredentialDto(latest.Id, latest.ValidUntil));
    }
}
