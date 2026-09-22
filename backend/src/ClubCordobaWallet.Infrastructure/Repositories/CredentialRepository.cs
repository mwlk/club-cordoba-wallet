using ClubCordobaWallet.Infrastructure.Persistence;

namespace ClubCordobaWallet.Infrastructure.Repositories;

public class CredentialRepository(AppDbContext db) : ICredentialRepository
{
    public async Task AddAsync(Credential credential, CancellationToken ct)
    {
        db.Credentials.Add(credential);
        await db.SaveChangesAsync(ct);
    }

    public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Credentials.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Credential>> GetAllAsync(CancellationToken ct) =>
        db.Credentials.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
}
