namespace ClubCordobaWallet.Application.Common;

// CQRS manual, sin MediatR (decisión documentada: menos boilerplate/magia
// para el alcance de esta prueba).
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken ct);
}

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken ct);
}
