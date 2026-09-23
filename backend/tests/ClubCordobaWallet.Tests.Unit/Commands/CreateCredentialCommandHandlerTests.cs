using ClubCordobaWallet.Application.Features.Credentials.Commands.CreateCredential;
using ClubCordobaWallet.Application.Interfaces;
using ClubCordobaWallet.Domain.Entities;
using ClubCordobaWallet.Domain.Enums;
using ClubCordobaWallet.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ClubCordobaWallet.Tests.Unit.Commands;

// Regla crítica de UC01 (extensión 5a): si falla la firma del Issuer, no se
// persiste nada. Estos tests usan fakes en memoria (sin librería de mocking,
// siguiendo el estilo ya usado en IssuerServiceTests) para verificar que,
// ante una falla de firma, nunca se llama IUnitOfWork.SaveChangesAsync.
public class CreateCredentialCommandHandlerTests
{
    private static CreateCredentialCommand SampleCommand(string dni = "30123456") => new(
        "Juan", "Pérez", dni, MemberCategory.adulto, "https://cdn.futbol.com.ar/socios/test.jpg");

    private class FakeMemberRepository : IMemberRepository
    {
        private readonly Member? _existing;
        public bool AddCalled { get; private set; }

        public FakeMemberRepository(Member? existing = null) => _existing = existing;

        public Task<Member?> GetByDniAsync(string dni, CancellationToken ct) => Task.FromResult(_existing);

        public Task<List<Member>> SearchByDniPrefixAsync(string dniPrefix, int take, CancellationToken ct) =>
            Task.FromResult(new List<Member>());

        public Task AddAsync(Member member, CancellationToken ct)
        {
            AddCalled = true;
            return Task.CompletedTask;
        }

        public Task<string> NextMemberNumberAsync(CancellationToken ct) => Task.FromResult("000123");
    }

    private class FakeCredentialRepository : ICredentialRepository
    {
        public bool AddCalled { get; private set; }

        public Task AddAsync(Credential credential, CancellationToken ct)
        {
            AddCalled = true;
            return Task.CompletedTask;
        }

        public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Credential?>(null);

        public Task<List<Credential>> GetAllAsync(CancellationToken ct) => Task.FromResult(new List<Credential>());
    }

    private class FailingIssuerService : IIssuerService
    {
        public Task<IssuedCredential> Issue(CredentialSubject subject, CredentialStatus status, CancellationToken ct)
            => throw new IssuerSigningException("Issuer:HmacKey no configurada.");
    }

    private class SucceedingIssuerService : IIssuerService
    {
        public Task<IssuedCredential> Issue(CredentialSubject subject, CredentialStatus status, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            return Task.FromResult(new IssuedCredential("{\"id\":\"https://credenciales.futbol.com.ar/test\"}", subject.NumeroSocio, now, now.AddYears(1)));
        }
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }
    }

    private class TenantServiceStub : ITenantService
    {
        public CredentialSubject BuildSubject(Member member, MemberCategory categoria, string foto) => new()
        {
            Id = member.Did,
            Nombre = member.FirstName,
            Apellido = member.LastName,
            Dni = member.Dni,
            NumeroSocio = member.MemberNumber,
            Categoria = categoria,
            Foto = foto
        };
    }

    [Fact]
    public async Task Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_New_Member()
    {
        var memberRepo = new FakeMemberRepository(existing: null);
        var credentialRepo = new FakeCredentialRepository();
        var unitOfWork = new FakeUnitOfWork();
        var sut = new CreateCredentialCommandHandler(
            memberRepo, credentialRepo, new TenantServiceStub(), new FailingIssuerService(), unitOfWork);

        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.ErrorKey.Should().Be("IssuerSigningFailed");
        // El Member se agrega al DbContext en memoria, pero nunca se commitea.
        memberRepo.AddCalled.Should().BeTrue();
        credentialRepo.AddCalled.Should().BeFalse();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_Not_Call_SaveChanges_When_Issuer_Fails_With_Existing_Member()
    {
        var existingMember = Member.Create("Juan", "Pérez", "30123456", "000123");
        var memberRepo = new FakeMemberRepository(existing: existingMember);
        var credentialRepo = new FakeCredentialRepository();
        var unitOfWork = new FakeUnitOfWork();
        var sut = new CreateCredentialCommandHandler(
            memberRepo, credentialRepo, new TenantServiceStub(), new FailingIssuerService(), unitOfWork);

        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        result.Success.Should().BeFalse();
        memberRepo.AddCalled.Should().BeFalse();
        credentialRepo.AddCalled.Should().BeFalse();
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_Call_SaveChanges_Once_When_Issuer_Succeeds_With_New_Member()
    {
        var memberRepo = new FakeMemberRepository(existing: null);
        var credentialRepo = new FakeCredentialRepository();
        var unitOfWork = new FakeUnitOfWork();
        var sut = new CreateCredentialCommandHandler(
            memberRepo, credentialRepo, new TenantServiceStub(), new SucceedingIssuerService(), unitOfWork);

        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        result.Success.Should().BeTrue();
        memberRepo.AddCalled.Should().BeTrue();
        credentialRepo.AddCalled.Should().BeTrue();
        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Reuse_Existing_Member_Did_And_Number_On_Success()
    {
        var existingMember = Member.Create("Juan", "Pérez", "30123456", "000123");
        var memberRepo = new FakeMemberRepository(existing: existingMember);
        var credentialRepo = new FakeCredentialRepository();
        var unitOfWork = new FakeUnitOfWork();
        var sut = new CreateCredentialCommandHandler(
            memberRepo, credentialRepo, new TenantServiceStub(), new SucceedingIssuerService(), unitOfWork);

        var result = await sut.Handle(SampleCommand(), CancellationToken.None);

        result.Success.Should().BeTrue();
        memberRepo.AddCalled.Should().BeFalse();
        result.Data!.MemberNumber.Should().Be(existingMember.MemberNumber);
        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }
}
