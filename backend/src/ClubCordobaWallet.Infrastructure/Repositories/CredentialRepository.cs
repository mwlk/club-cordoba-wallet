using ClubCordobaWallet.Infrastructure.Persistence;

namespace ClubCordobaWallet.Infrastructure.Repositories;

public class CredentialRepository(AppDbContext db) : ICredentialRepository
{
    // Sin commit propio -> el commit único lo decide el CommandHandler vía
    // IUnitOfWork, después de confirmar el éxito del Issuer.
    public Task AddAsync(Credential credential, CancellationToken ct)
    {
        db.Credentials.Add(credential);
        return Task.CompletedTask;
    }

    public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Credentials.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Credential>> GetAllAsync(CancellationToken ct) =>
        db.Credentials.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
}
