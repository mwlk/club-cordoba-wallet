using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ClubCordobaWallet.Domain.Exceptions;
using Microsoft.Extensions.Configuration;

namespace ClubCordobaWallet.Infrastructure.Services;

// Servicio in-process del Issuer (sin endpoint propio, tal cual 4.2.1).
// Responsable de: agregar id/type/issuer/validFrom/validUntil, serializar
// el JSON canónico EXACTO que pide el enunciado (4.1.2), firmarlo con
// HMAC-SHA256 y devolver la VC completa con proof.
public class IssuerService(IConfiguration configuration) : IIssuerService
{
    // Encoder sin escape de unicode: "ñ"/"é" deben viajar tal cual en el
    // JSON canónico, o la firma no reproduce el ejemplo del enunciado.
    private static readonly JsonSerializerOptions CanonicalOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    public Task<IssuedCredential> Issue(CredentialSubject subject, CredentialStatus status, CancellationToken ct)
    {
        var issuerDid = configuration["Issuer:Did"];
        var verificationMethod = configuration["Issuer:VerificationMethod"];
        var hmacKey = configuration["Issuer:HmacKey"];

        if (string.IsNullOrWhiteSpace(hmacKey))
            throw new IssuerSigningException("Issuer:HmacKey is not configured.");

        // Un único timestamp truncado a segundos, reutilizado para
        // validFrom Y proof.created -> "en la práctica coincide con
        // validFrom" (enunciado 4.1.2, subcampos de proof).
        var now = DateTime.UtcNow;
        var validFrom = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
        var validUntil = validFrom.AddYears(1);

        var vcId = $"https://credenciales.futbol.com.ar/{Guid.NewGuid()}";

        // credentialSubject en orden alfabético ORDINAL (StringComparer.Ordinal),
        // exactamente como especifica el enunciado.
        var subjectNode = new JsonObject
        {
            ["apellido"] = subject.Apellido,
            ["categoria"] = subject.Categoria.ToString(),
            ["dni"] = subject.Dni,
            ["foto"] = subject.Foto,
            ["id"] = subject.Id,
            ["nombre"] = subject.Nombre,
            ["numeroSocio"] = subject.NumeroSocio
        };

        // VC sin proof, claves de primer nivel en orden alfabético ordinal.
        var vcWithoutProof = new JsonObject
        {
            ["credentialStatus"] = (int)status,
            ["credentialSubject"] = subjectNode,
            ["id"] = vcId,
            ["issuer"] = issuerDid,
            ["type"] = new JsonArray("VerifiableCredential", "SocioCredential"),
            ["validFrom"] = FormatIso(validFrom),
            ["validUntil"] = FormatIso(validUntil)
        };

        var canonicalJson = vcWithoutProof.ToJsonString(CanonicalOptions);
        var proofValue = ComputeHmac(canonicalJson, hmacKey);

        var proofNode = new JsonObject
        {
            ["type"] = "HMAC-SHA256",
            ["created"] = FormatIso(validFrom),
            ["verificationMethod"] = verificationMethod,
            ["proofValue"] = proofValue
        };

        // VC completa, con proof agregado, reordenada alfabéticamente de
        // nuevo (incluyendo "proof") para que quede persistida en orden
        // consistente. La firma ya se calculó sobre el JSON SIN proof,
        // como pide la spec -> este reordenado final no la afecta.
        var fullVc = new JsonObject
        {
            ["credentialStatus"] = (int)status,
            ["credentialSubject"] = subjectNode.DeepClone(),
            ["id"] = vcId,
            ["issuer"] = issuerDid,
            ["proof"] = proofNode,
            ["type"] = new JsonArray("VerifiableCredential", "SocioCredential"),
            ["validFrom"] = FormatIso(validFrom),
            ["validUntil"] = FormatIso(validUntil)
        };

        var vcJson = fullVc.ToJsonString(CanonicalOptions);

        return Task.FromResult(new IssuedCredential(vcJson, subject.NumeroSocio, validFrom, validUntil));
    }

    private static string FormatIso(DateTime dt) => dt.ToString("yyyy-MM-ddTHH:mm:ssZ");

    private static string ComputeHmac(string canonicalJson, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalJson));
        return Convert.ToBase64String(hash);
    }
}
