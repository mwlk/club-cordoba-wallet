namespace ClubCordobaWallet.Application.Features.Credentials.Dtos;

// Detalle para GET /credentials/{id}: se muestra como "carnet" en el
// frontend. Incluye los campos técnicos del protocolo por separado
// (issuer/proof) para la sección colapsable "detalles de seguridad",
// nunca como JSON crudo -> ver docs/decisiones.md (UX del admin).
public record CredentialDetailDto(
    Guid Id,
    string Photo,
    string FirstName,
    string LastName,
    MemberCategory Category,
    string MemberNumber,
    DateTime ValidFrom,
    DateTime ValidUntil,
    CredentialStatus Status,
    string Issuer,
    string ProofType
);
