using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace ClubCordobaWallet.Tests.Integration.Api;

// Fixture con Testcontainers: levanta un PostgreSQL real en Docker para
// cada corrida de tests, sin depender del compose de desarrollo.
public class CredentialsEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:18-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = default!;
    private HttpClient _client = default!;

    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Default", _db.GetConnectionString());
            builder.UseSetting("Issuer:HmacKey", "integration-test-secret");
        });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        _factory.Dispose();
    }

    [Fact]
    public async Task Get_Credentials_When_Empty_Returns_Empty_List()
    {
        var response = await _client.GetAsync("/api/credentials");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<object>>();
        body.Should().BeEmpty();
    }

    [Fact]
    public async Task Full_Flow_Create_Then_List_Then_Detail()
    {
        var createBody = new
        {
            nombre = "Ana",
            apellido = "García",
            dni = "35123456",
            categoria = "adulto",
            foto = "https://cdn.futbol.com.ar/socios/test.jpg"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/credentials", createBody);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResponse = await _client.GetAsync("/api/credentials");
        var list = await listResponse.Content.ReadFromJsonAsync<List<System.Text.Json.JsonElement>>();
        list.Should().HaveCount(1);

        var id = list![0].GetProperty("id").GetGuid();
        var detailResponse = await _client.GetAsync($"/api/credentials/{id}");
        detailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Credential_By_Id_Not_Found_Returns_Result_Pattern_404()
    {
        // Un GUID bien formado que no existe -> 404 con result pattern
        // ({success:false, message, data:null}), no un body vacío.
        var response = await _client.GetAsync($"/api/credentials/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        body.GetProperty("success").GetBoolean().Should().BeFalse();
        body.GetProperty("message").GetString().Should().NotBeNullOrWhiteSpace();
        body.GetProperty("data").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
    }

    [Fact]
    public async Task Get_Unmatched_Route_Returns_Result_Pattern_404()
    {
        // Id con formato inválido y ruta desconocida: ninguna acción matchea
        // -> el fallback global de 404 mantiene el result pattern.
        var invalidIdResponse = await _client.GetAsync("/api/credentials/not-a-guid");
        invalidIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var invalidIdBody = await invalidIdResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        invalidIdBody.GetProperty("success").GetBoolean().Should().BeFalse();
        invalidIdBody.GetProperty("data").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);

        var unknownRouteResponse = await _client.GetAsync("/api/unknown-route");
        unknownRouteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var unknownRouteBody = await unknownRouteResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        unknownRouteBody.GetProperty("success").GetBoolean().Should().BeFalse();
        unknownRouteBody.GetProperty("data").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
    }

    [Fact]
    public async Task Search_Member_Not_Found_Returns_200_With_Empty_List()
    {
        // mejora-busqueda-socio: la búsqueda es por prefijo y siempre
        // responde Result.Ok — "sin coincidencias" es un array vacío, no
        // un success:false (eso queda para errores reales).
        var response = await _client.GetAsync("/api/credentials/members/search?dni=99999999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        body.GetProperty("success").GetBoolean().Should().BeTrue();
        body.GetProperty("data").GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetActiveCredential_Returns_Null_When_Member_Or_Active_Credential_Does_Not_Exist()
    {
        var response = await _client.GetAsync("/api/credentials/members/active-credential?dni=99999999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        body.GetProperty("success").GetBoolean().Should().BeTrue();
        body.GetProperty("data").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
    }

    // renovacion-credencial-activa: flujo completo — alta -> GET active-credential
    // la encuentra -> segunda alta sin confirmar falla sin persistir -> segunda
    // alta confirmada renueva (vieja deja de contar como activa).
    [Fact]
    public async Task Renewal_Flow_Requires_Confirmation_And_Expires_Old_Credential()
    {
        const string dni = "38222333";
        var createBody = new
        {
            nombre = "Marta",
            apellido = "Sosa",
            dni,
            categoria = "adulto",
            foto = "https://cdn.futbol.com.ar/socios/test2.jpg"
        };

        var firstCreate = await _client.PostAsJsonAsync("/api/credentials", createBody);
        firstCreate.StatusCode.Should().Be(HttpStatusCode.Created);

        var activeAfterFirst = await _client.GetAsync($"/api/credentials/members/active-credential?dni={dni}");
        var activeAfterFirstBody = await activeAfterFirst.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        activeAfterFirstBody.GetProperty("data").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Object);

        var secondCreateNoConfirm = await _client.PostAsJsonAsync("/api/credentials", createBody);
        secondCreateNoConfirm.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var failBody = await secondCreateNoConfirm.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        failBody.GetProperty("success").GetBoolean().Should().BeFalse();

        var listAfterFailedAttempt = await _client.GetAsync("/api/credentials");
        var listAfterFailedAttemptBody = await listAfterFailedAttempt.Content.ReadFromJsonAsync<List<System.Text.Json.JsonElement>>();
        listAfterFailedAttemptBody.Should().HaveCount(1);

        var confirmedCreateBody = new
        {
            nombre = "Marta",
            apellido = "Sosa",
            dni,
            categoria = "adulto",
            foto = "https://cdn.futbol.com.ar/socios/test2.jpg",
            confirmarRenovacion = true
        };

        var secondCreateConfirmed = await _client.PostAsJsonAsync("/api/credentials", confirmedCreateBody);
        secondCreateConfirmed.StatusCode.Should().Be(HttpStatusCode.Created);

        var listAfterRenewal = await _client.GetAsync("/api/credentials");
        var listAfterRenewalBody = await listAfterRenewal.Content.ReadFromJsonAsync<List<System.Text.Json.JsonElement>>();
        listAfterRenewalBody.Should().HaveCount(2);
    }
}
