namespace ClubCordobaWallet.Application.Features.Credentials.Commands.CreateCredential;

// Los 5 campos del formulario de alta, tal cual la sección 4.1.3 del
// enunciado. Sin memberId: el buscador de socio en el frontend es solo
// UX (autocompletar), el alta real del socio -si hace falta- ocurre acá
// adentro, de forma transparente. Ver docs/decisiones.md.
public record CreateCredentialCommand(
    string Nombre,
    string Apellido,
    string Dni,
    MemberCategory Categoria,
    string Foto
);
