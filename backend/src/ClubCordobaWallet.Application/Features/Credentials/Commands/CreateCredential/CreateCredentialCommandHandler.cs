using ClubCordobaWallet.Application.Features.Credentials.Dtos;
using ClubCordobaWallet.Domain.Exceptions;

namespace ClubCordobaWallet.Application.Features.Credentials.Commands.CreateCredential;

public class CreateCredentialCommandHandler(
    IMemberRepository memberRepository,
    ICredentialRepository credentialRepository,
    ITenantService tenantService,
    IIssuerService issuerService,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateCredentialCommand, Result<CreateCredentialResult>>
{
    public async Task<Result<CreateCredentialResult>> Handle(CreateCredentialCommand command, CancellationToken ct)
    {
        // 1. Reutilizar socio existente por DNI, o crear uno nuevo.
        //    "se genera una vez y se persiste" (enunciado 4.1.2) -> el DID
        //    y el numeroSocio viven en Member, no se regeneran en cada alta.
        //    AddAsync solo agrega al DbContext, sin commit (ver IUnitOfWork).
        var member = await memberRepository.GetByDniAsync(command.Dni, ct);
        if (member is null)
        {
            var memberNumber = await memberRepository.NextMemberNumberAsync(ct);
            member = Member.Create(command.Nombre, command.Apellido, command.Dni, memberNumber);
            await memberRepository.AddAsync(member, ct);
        }

        // 2. renovacion-credencial-activa: si el socio ya tiene alguna
        //    credencial vigente, hace falta confirmación explícita para
        //    emitir una nueva (evita acumular credenciales "activas"
        //    simultáneas). El popup del frontend es la UX, pero acá es
        //    donde se hace cumplir la regla -> un cliente que no pase por
        //    el popup (curl, otro frontend) no puede saltearse esto.
        var activeCredentials = await credentialRepository.GetActiveByMemberIdAsync(member.Id, ct);
        if (activeCredentials.Count > 0 && !command.ConfirmarRenovacion)
        {
            // Nada se persiste: si el member era nuevo, su Add sigue sin
            // commitear (mismo mecanismo que la falla de firma).
            return Result<CreateCredentialResult>.Fail("ActiveCredentialExists");
        }

        // 3. Armar el credentialSubject (rol Tenant).
        var subject = tenantService.BuildSubject(member, command.Categoria, command.Foto);

        // 4. Invocar al Issuer. Si falla la firma, no se persiste nada
        //    (requisito explícito del enunciado, extensión 5a de UC01):
        //    como todavía no se llamó unitOfWork.SaveChangesAsync, el Member
        //    recién agregado (si aplica) nunca llega a la base.
        IssuedCredential issued;
        try
        {
            issued = await issuerService.Issue(subject, CredentialStatus.active, ct);
        }
        catch (IssuerSigningException)
        {
            return Result<CreateCredentialResult>.Fail("IssuerSigningFailed");
        }

        // 5. renovacion-credencial-activa: recién con el Issuer confirmado
        //    se marcan las viejas como expiradas -> si el Issuer hubiera
        //    fallado en el paso anterior, ninguna se toca (siguen vigentes).
        foreach (var old in activeCredentials)
        {
            old.ExpireNow();
            credentialRepository.Update(old);
        }

        // 6. Persistir la VC completa. Commit único: Member (si es nuevo) +
        //    expiración de las viejas (si aplica) + Credential nueva se
        //    guardan juntos, recién acá.
        var credential = Credential.Create(member.Id, issued.VcJson);
        await credentialRepository.AddAsync(credential, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<CreateCredentialResult>.Ok(
            new CreateCredentialResult(member.MemberNumber, issued.ValidFrom, issued.ValidUntil),
            "CredentialIssued");
    }
}
