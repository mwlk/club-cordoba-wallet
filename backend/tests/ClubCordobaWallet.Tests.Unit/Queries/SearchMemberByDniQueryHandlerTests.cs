using ClubCordobaWallet.Application.Features.Credentials.Queries.SearchMemberByDni;
using ClubCordobaWallet.Application.Interfaces;
using ClubCordobaWallet.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ClubCordobaWallet.Tests.Unit.Queries;

// mejora-busqueda-socio: la búsqueda por prefijo de DNI siempre devuelve
// Result.Ok con una lista (0..N), nunca Fail — "sin coincidencias" es un
// resultado válido (socio nuevo), no un error.
public class SearchMemberByDniQueryHandlerTests
{
    private class FakeMemberRepository : IMemberRepository
    {
        private readonly List<Member> _members;
        public FakeMemberRepository(params Member[] members) => _members = members.ToList();

        public Task<Member?> GetByDniAsync(string dni, CancellationToken ct) =>
            Task.FromResult(_members.FirstOrDefault(m => m.Dni == dni));

        public Task<List<Member>> SearchByDniPrefixAsync(string dniPrefix, int take, CancellationToken ct) =>
            Task.FromResult(_members.Where(m => m.Dni.StartsWith(dniPrefix)).Take(take).ToList());

        public Task AddAsync(Member member, CancellationToken ct) => Task.CompletedTask;

        public Task<string> NextMemberNumberAsync(CancellationToken ct) => Task.FromResult("000001");
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Match()
    {
        var handler = new SearchMemberByDniQueryHandler(new FakeMemberRepository());

        var result = await handler.Handle(new SearchMemberByDniQuery("40199"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Return_Single_Candidate_When_One_Match()
    {
        var member = Member.Create("Juan", "Pérez", "40199111", "000001");
        var handler = new SearchMemberByDniQueryHandler(new FakeMemberRepository(member));

        var result = await handler.Handle(new SearchMemberByDniQuery("40199"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().ContainSingle();
        result.Data![0].Dni.Should().Be("40199111");
    }

    [Fact]
    public async Task Handle_Should_Return_Multiple_Candidates_When_Several_Match()
    {
        var m1 = Member.Create("Juan", "Pérez", "40199111", "000001");
        var m2 = Member.Create("Ana", "Gómez", "40199222", "000002");
        var handler = new SearchMemberByDniQueryHandler(new FakeMemberRepository(m1, m2));

        var result = await handler.Handle(new SearchMemberByDniQuery("40199"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }
}
