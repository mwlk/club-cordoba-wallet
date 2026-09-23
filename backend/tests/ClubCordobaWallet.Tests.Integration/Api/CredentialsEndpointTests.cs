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
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
        .WithImage("postgres:18-alpine")
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
}
