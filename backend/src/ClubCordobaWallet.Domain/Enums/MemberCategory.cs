using System.Text.Json.Serialization;

namespace ClubCordobaWallet.Domain.Enums;

// Nombres en minúscula deliberadamente: coinciden 1:1 con los valores
// que exige el enunciado dentro del credentialSubject ("adulto", "juvenil", "niño").
// Al serializar con JsonStringEnumConverter, el nombre del enum ES el valor JSON,
// evitando mapeos manuales que puedan romper la firma canónica.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemberCategory
{
    adulto,
    juvenil,
    niño
}
