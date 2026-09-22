namespace ClubCordobaWallet.Domain.Exceptions;

// Se lanza cuando el Issuer no puede firmar (ej: falta la clave HMAC).
// El enunciado exige explícitamente: si falla la firma, no se persiste nada.
public class IssuerSigningException(string message) : Exception(message);
