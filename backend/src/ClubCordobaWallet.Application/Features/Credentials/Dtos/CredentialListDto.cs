namespace ClubCordobaWallet.Application.Features.Credentials.Dtos;

// DTO mínimo para el listado (UC02): "foto, nombre, apellido, categoría,
// número de socio, vigencia y estado". Nunca se devuelve la VC completa acá.
public record CredentialListDto(
    Guid Id,
    string Photo,
    string FirstName,
    string LastName,
    MemberCategory Category,
    string MemberNumber,
    DateTime ValidFrom,
    DateTime ValidUntil,
    CredentialStatus Status
);
