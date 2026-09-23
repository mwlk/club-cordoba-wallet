using ClubCordobaWallet.Application.Features.Credentials.Queries.GetActiveCredentialByDni;
using ClubCordobaWallet.Application.Interfaces;
using ClubCordobaWallet.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ClubCordobaWallet.Tests.Unit.Queries;

// renovacion-credencial-activa: consulta previa usada por el popup de
// confirmación del alta. Siempre Result.Ok -> "sin credencial vigente" es
// un resultado válido (data: null), no un error.
public class GetActiveCredentialByDniQueryHandlerTests
{
    private class FakeMemberRepository : IMemberRepository
    {
        private readonly Member? _member;
        public FakeMemberRepository(Member? member = null) => _member = member;

        public Task<Member?> GetByDniAsync(string dni, CancellationToken ct) =>
            Task.FromResult(_member is not null && _member.Dni == dni ? _member : null);

        public Task<List<Member>> SearchByDniPrefixAsync(string dniPrefix, int take, CancellationToken ct) =>
            Task.FromResult(new List<Member>());

        public Task AddAsync(Member member, CancellationToken ct) => Task.CompletedTask;

        public Task<string> NextMemberNumberAsync(CancellationToken ct) => Task.FromResult("000001");
    }

    private class FakeCredentialRepository : ICredentialRepository
    {
        private readonly List<Credential> _active;
        public FakeCredentialRepository(List<Credential>? active = null) => _active = active ?? new List<Credential>();

        public Task AddAsync(Credential credential, CancellationToken ct) => Task.CompletedTask;
        public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Credential?>(null);
        public Task<List<Credential>> GetAllAsync(CancellationToken ct) => Task.FromResult(new List<Credential>());
        public Task<List<Credential>> GetActiveByMemberIdAsync(Guid memberId, CancellationToken ct) => Task.FromResult(_active);
        public void Update(Credential credential) { }
    }

    private static Credential Active(Guid memberId, DateTime validUntil) =>
        Credential.Create(memberId, $"{{\"validUntil\":\"{validUntil:yyyy-MM-ddTHH:mm:ssZ}\"}}");

    [Fact]
    public async Task Handle_Should_Return_Null_When_Member_Does_Not_Exist()
    {
        var handler = new GetActiveCredentialByDniQueryHandler(new FakeMemberRepository(), new FakeCredentialRepository());

        var result = await handler.Handle(new GetActiveCredentialByDniQuery("99999999"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Member_Has_No_Credentials()
    {
        var member = Member.Create("Juan", "Pérez", "30123456", "000001");
        var handler = new GetActiveCredentialByDniQueryHandler(
            new FakeMemberRepository(member), new FakeCredentialRepository());

        var result = await handler.Handle(new GetActiveCredentialByDniQuery("30123456"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Only_Expired_Credentials()
    {
        var member = Member.Create("Juan", "Pérez", "30123456", "000001");
        // GetActiveByMemberIdAsync ya filtra vencidas -> simulamos que no devuelve ninguna.
        var handler = new GetActiveCredentialByDniQueryHandler(
            new FakeMemberRepository(member), new FakeCredentialRepository(new List<Credential>()));

        var result = await handler.Handle(new GetActiveCredentialByDniQuery("30123456"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_ValidUntil_When_Active_Credential_Exists()
    {
        var member = Member.Create("Juan", "Pérez", "30123456", "000001");
        var validUntil = DateTime.UtcNow.AddMonths(6);
        var credentialRepo = new FakeCredentialRepository(new List<Credential> { Active(member.Id, validUntil) });
        var handler = new GetActiveCredentialByDniQueryHandler(new FakeMemberRepository(member), credentialRepo);

        var result = await handler.Handle(new GetActiveCredentialByDniQuery("30123456"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ValidUntil.Should().BeCloseTo(validUntil, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Handle_Should_Return_Furthest_ValidUntil_When_Multiple_Active_Credentials()
    {
        // sdd-review renovacion-credencial-activa (WARNING 2): design.md
        // decide que, si hubiera más de una activa (condición de carrera
        // aceptada), se informa la de validUntil más lejano.
        var member = Member.Create("Juan", "Pérez", "30123456", "000001");
        var soonest = DateTime.UtcNow.AddMonths(1);
        var furthest = DateTime.UtcNow.AddMonths(6);
        var credentialRepo = new FakeCredentialRepository(new List<Credential>
        {
            Active(member.Id, soonest),
            Active(member.Id, furthest)
        });
        var handler = new GetActiveCredentialByDniQueryHandler(new FakeMemberRepository(member), credentialRepo);

        var result = await handler.Handle(new GetActiveCredentialByDniQuery("30123456"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ValidUntil.Should().BeCloseTo(furthest, TimeSpan.FromSeconds(1));
    }
}
