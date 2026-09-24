namespace ClubCordobaWallet.Application.Interfaces;

// Commit único explícito. Los repositorios solo hacen Add/consultas; quien
// decide CUÁNDO se persiste es el CommandHandler, después de confirmar el
// éxito del Issuer -> si falla la firma, nunca se llama SaveChangesAsync y
// nada queda persistido (UC01, extensión 5a). Ver docs/decisiones.md.
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
