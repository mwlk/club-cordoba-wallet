using System.Text.Json;
using ClubCordobaWallet.Application.Interfaces;
using ClubCordobaWallet.Domain.Enums;
using ClubCordobaWallet.Domain.Exceptions;
using ClubCordobaWallet.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ClubCordobaWallet.Tests.Unit.Services;

// El servicio más crítico del sistema: si la canonicalización o el
// encoding fallan, la firma no es reproducible. Estos tests validan
// el contrato exacto que pide el enunciado (4.1.2).
public class IssuerServiceTests
{
    private static IConfiguration BuildConfig(string? hmacKey = "test-secret-key") =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Issuer:Did"] = "did:example:futbol",
            ["Issuer:VerificationMethod"] = "did:example:futbol#key-1",
            ["Issuer:HmacKey"] = hmacKey
        }).Build();

    private static CredentialSubject SampleSubject() => new()
    {
        Id = "did:example:3fa85f64-5717-4562-b3fc-2c963f66afa6",
        Nombre = "Juan",
        Apellido = "Pérez",
        Dni = "30123456",
        NumeroSocio = "000123",
        Categoria = MemberCategory.adulto,
        Foto = "https://cdn.futbol.com.ar/socios/8f14e45f.jpg"
    };

    [Fact]
    public async Task Issue_Should_Produce_Vc_With_Alphabetically_Ordered_Keys()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        var doc = JsonDocument.Parse(result.VcJson).RootElement;
        var topLevelKeys = doc.EnumerateObject().Select(p => p.Name).ToList();
        topLevelKeys.Should().BeInAscendingOrder(StringComparer.Ordinal);

        var subjectKeys = doc.GetProperty("credentialSubject").EnumerateObject().Select(p => p.Name).ToList();
        subjectKeys.Should().BeInAscendingOrder(StringComparer.Ordinal);
    }

    [Fact]
    public async Task Issue_Should_Not_Escape_Non_Ascii_Characters()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        // "Pérez" y "adulto"/"niño" no deben quedar escapados como \u00XX,
        // o la firma no sería reproducible según el ejemplo del enunciado.
        result.VcJson.Should().Contain("Pérez");
        result.VcJson.Should().NotContain("\\u00");
    }

    [Fact]
    public async Task Issue_Should_Produce_Base64_ProofValue()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        var doc = JsonDocument.Parse(result.VcJson).RootElement;
        var proofValue = doc.GetProperty("proof").GetProperty("proofValue").GetString();

        proofValue.Should().NotBeNullOrEmpty();
        var act = () => Convert.FromBase64String(proofValue!);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Issue_Should_Use_Same_Timestamp_For_ValidFrom_And_ProofCreated()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        var doc = JsonDocument.Parse(result.VcJson).RootElement;
        var validFrom = doc.GetProperty("validFrom").GetString();
        var proofCreated = doc.GetProperty("proof").GetProperty("created").GetString();

        validFrom.Should().Be(proofCreated);
    }

    [Fact]
    public async Task Issue_Should_Format_Dates_Without_Milliseconds()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        var doc = JsonDocument.Parse(result.VcJson).RootElement;
        var validFrom = doc.GetProperty("validFrom").GetString();

        validFrom.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$");
    }

    [Fact]
    public async Task Issue_Should_Set_ValidUntil_One_Year_After_ValidFrom()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        result.ValidUntil.Should().Be(result.ValidFrom.AddYears(1));
    }

    [Fact]
    public async Task Issue_Should_Throw_When_HmacKey_Is_Missing()
    {
        var sut = new IssuerService(BuildConfig(hmacKey: null));

        var act = async () => await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        await act.Should().ThrowAsync<IssuerSigningException>();
    }

    [Fact]
    public async Task Issue_Should_Serialize_CredentialStatus_As_Integer()
    {
        var sut = new IssuerService(BuildConfig());

        var result = await sut.Issue(SampleSubject(), CredentialStatus.active, CancellationToken.None);

        var doc = JsonDocument.Parse(result.VcJson).RootElement;
        doc.GetProperty("credentialStatus").ValueKind.Should().Be(JsonValueKind.Number);
        doc.GetProperty("credentialStatus").GetInt32().Should().Be(0);
    }
}
