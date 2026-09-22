namespace ClubCordobaWallet.Application.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetByDniAsync(string dni, CancellationToken ct);
    Task AddAsync(Member member, CancellationToken ct);
    Task<string> NextMemberNumberAsync(CancellationToken ct);
}
