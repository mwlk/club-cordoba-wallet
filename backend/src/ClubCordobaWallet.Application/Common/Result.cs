namespace ClubCordobaWallet.Application.Common;

// Result pattern: toda operación de negocio devuelve esto en vez de tirar
// excepciones para casos esperados (socio no encontrado, DNI duplicado, etc).
// MessageKey/ErrorKey son claves de resx (Messages.resx / Errors.resx),
// no el texto final -> el controller resuelve el string localizado.
public class Result<T>
{
    public bool Success { get; }
    public T? Data { get; }
    public string? MessageKey { get; }
    public string? ErrorKey { get; }

    private Result(bool success, T? data, string? messageKey, string? errorKey)
    {
        Success = success;
        Data = data;
        MessageKey = messageKey;
        ErrorKey = errorKey;
    }

    public static Result<T> Ok(T data, string? messageKey = null) => new(true, data, messageKey, null);
    public static Result<T> Fail(string errorKey) => new(false, default, null, errorKey);
}
