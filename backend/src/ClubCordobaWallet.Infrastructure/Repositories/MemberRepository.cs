using ClubCordobaWallet.Infrastructure.Persistence;

namespace ClubCordobaWallet.Infrastructure.Repositories;

public class MemberRepository(AppDbContext db) : IMemberRepository
{
    public Task<Member?> GetByDniAsync(string dni, CancellationToken ct) =>
        db.Members.FirstOrDefaultAsync(m => m.Dni == dni, ct);

    public Task<List<Member>> SearchByDniPrefixAsync(string dniPrefix, int take, CancellationToken ct) =>
        db.Members
            .Where(m => EF.Functions.Like(m.Dni, dniPrefix + "%"))
            .OrderBy(m => m.Dni)
            .Take(take)
            .ToListAsync(ct);

    // Sin commit propio -> el commit único lo decide el CommandHandler vía
    // IUnitOfWork, después de confirmar el éxito del Issuer.
    public Task AddAsync(Member member, CancellationToken ct)
    {
        db.Members.Add(member);
        return Task.CompletedTask;
    }

    public async Task<string> NextMemberNumberAsync(CancellationToken ct)
    {
        // nextval sobre la secuencia de PG, formateado a 6 dígitos
        // zero-padded (formato del ejemplo del enunciado: "000123").
        var next = await db.Database
            .SqlQuery<long>($"SELECT nextval('member_number_seq') AS \"Value\"")
            .SingleAsync(ct);

        return next.ToString("D6");
    }
}
