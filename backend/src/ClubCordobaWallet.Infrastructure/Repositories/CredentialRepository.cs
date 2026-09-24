using System.Text.Json;
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

    // renovacion-credencial-activa: validUntil vive dentro de vc_json
    // (JSONB), no es columna -> se trae todo lo del member y se filtra acá,
    // mismo patrón que GetCredentialsQueryHandler. Puede haber más de una
    // activa (condición de carrera aceptada, ver design.md) -> se devuelven
    // todas, no solo la última.
    public async Task<List<Credential>> GetActiveByMemberIdAsync(Guid memberId, CancellationToken ct)
    {
        var all = await db.Credentials.Where(c => c.MemberId == memberId).ToListAsync(ct);
        var now = DateTime.UtcNow;

        return all
            .Where(c => JsonDocument.Parse(c.VcJson).RootElement.GetProperty("validUntil").GetDateTime() > now)
            .ToList();
    }

    // Sin commit propio -> el commit único lo decide el CommandHandler vía
    // IUnitOfWork. La entidad ya está trackeada (vino de este mismo
    // DbContext), pero se marca explícito para no depender de detección
    // automática de cambios.
    public void Update(Credential credential) => db.Credentials.Update(credential);
}
