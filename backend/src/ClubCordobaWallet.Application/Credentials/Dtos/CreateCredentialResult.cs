namespace ClubCordobaWallet.Application.Credentials.Dtos;

// Lo mínimo que pide el enunciado mostrar tras el alta: número de socio y
// vigencia (sección 4.1.3). Nunca la VC completa.
public record CreateCredentialResult(string MemberNumber, DateTime ValidFrom, DateTime ValidUntil);
