namespace ClubCordobaWallet.Application.Features.Credentials.Dtos;

// renovacion-credencial-activa: usado tanto por la consulta previa (popup
// de confirmación en el alta) como por el listado interno del handler de
// alta para decidir si hace falta confirmar.
public record ActiveCredentialDto(Guid CredentialId, DateTime ValidUntil);
