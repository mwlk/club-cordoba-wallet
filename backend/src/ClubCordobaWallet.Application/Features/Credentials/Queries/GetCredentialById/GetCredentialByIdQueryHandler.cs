using System.Text.Json;
using ClubCordobaWallet.Application.Features.Credentials.Dtos;

namespace ClubCordobaWallet.Application.Features.Credentials.Queries.GetCredentialById;

public class GetCredentialByIdQueryHandler(ICredentialRepository credentialRepository)
    : IQueryHandler<GetCredentialByIdQuery, Result<CredentialDetailDto>>
{
    public async Task<Result<CredentialDetailDto>> Handle(GetCredentialByIdQuery query, CancellationToken ct)
    {
        var credential = await credentialRepository.GetByIdAsync(query.Id, ct);
        if (credential is null)
            return Result<CredentialDetailDto>.Fail("CredentialNotFound");

        var vc = JsonDocument.Parse(credential.VcJson).RootElement;
        var subject = vc.GetProperty("credentialSubject");

        var dto = new CredentialDetailDto(
            credential.Id,
            subject.GetProperty("foto").GetString()!,
            subject.GetProperty("nombre").GetString()!,
            subject.GetProperty("apellido").GetString()!,
            Enum.Parse<MemberCategory>(subject.GetProperty("categoria").GetString()!),
            subject.GetProperty("numeroSocio").GetString()!,
            vc.GetProperty("validFrom").GetDateTime(),
            vc.GetProperty("validUntil").GetDateTime(),
            (CredentialStatus)vc.GetProperty("credentialStatus").GetInt32(),
            vc.GetProperty("issuer").GetString()!,
            vc.GetProperty("proof").GetProperty("type").GetString()!
        );

        return Result<CredentialDetailDto>.Ok(dto);
    }
}
