namespace ClubCordobaWallet.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Credential> Credentials => Set<Credential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Secuencia para member_number. Se acepta que queden gaps si el
        // alta falla luego de tomar el nextval (las secuencias de PG no
        // son transaccionales) -> documentado en docs/decisiones.md.
        modelBuilder.HasSequence<long>("member_number_seq").StartsAt(1).IncrementsBy(1);
    }
}
