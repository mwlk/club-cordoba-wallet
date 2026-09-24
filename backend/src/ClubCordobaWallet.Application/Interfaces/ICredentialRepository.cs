namespace ClubCordobaWallet.Application.Interfaces;

public interface ICredentialRepository
{
    Task AddAsync(Credential credential, CancellationToken ct);
    Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Credential>> GetAllAsync(CancellationToken ct);
    Task<List<Credential>> GetActiveByMemberIdAsync(Guid memberId, CancellationToken ct);
    void Update(Credential credential);
}
