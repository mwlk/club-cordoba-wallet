using ClubCordobaWallet.Application.Credentials.Dtos;
using ClubCordobaWallet.Domain.Exceptions;

namespace ClubCordobaWallet.Application.Credentials.Commands.CreateCredential;

public class CreateCredentialCommandHandler(
    IMemberRepository memberRepository,
    ICredentialRepository credentialRepository,
    ITenantService tenantService,
    IIssuerService issuerService
) : ICommandHandler<CreateCredentialCommand, Result<CreateCredentialResult>>
{
    public async Task<Result<CreateCredentialResult>> Handle(CreateCredentialCommand command, CancellationToken ct)
    {
        // 1. Reutilizar socio existente por DNI, o crear uno nuevo.
        //    "se genera una vez y se persiste" (enunciado 4.1.2) -> el DID
        //    y el numeroSocio viven en Member, no se regeneran en cada alta.
        var member = await memberRepository.GetByDniAsync(command.Dni, ct);
        if (member is null)
        {
            var memberNumber = await memberRepository.NextMemberNumberAsync(ct);
            member = Member.Create(command.Nombre, command.Apellido, command.Dni, memberNumber);
            await memberRepository.AddAsync(member, ct);
        }

        // 2. Armar el credentialSubject (rol Tenant).
        var subject = tenantService.BuildSubject(member, command.Categoria, command.Foto);

        // 3. Invocar al Issuer. Si falla la firma, no se persiste nada
        //    (requisito explícito del enunciado, extensión 5a de UC01).
        IssuedCredential issued;
        try
        {
            issued = await issuerService.Issue(subject, CredentialStatus.active, ct);
        }
        catch (IssuerSigningException)
        {
            return Result<CreateCredentialResult>.Fail("IssuerSigningFailed");
        }

        // 4. Persistir la VC completa.
        var credential = Credential.Create(member.Id, issued.VcJson);
        await credentialRepository.AddAsync(credential, ct);

        return Result<CreateCredentialResult>.Ok(
            new CreateCredentialResult(member.MemberNumber, issued.ValidFrom, issued.ValidUntil),
            "CredentialIssued");
    }
}
