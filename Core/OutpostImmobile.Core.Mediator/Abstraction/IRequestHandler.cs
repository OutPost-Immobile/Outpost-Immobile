namespace OutpostImmobile.Core.Mediator.Abstraction;

public interface IRequestHandler<TQuery, TResponse>
{
    public TResponse Handle(TQuery query, CancellationToken cancellationToken);
}