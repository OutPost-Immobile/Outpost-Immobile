namespace OutpostImmobile.Core.Mediator.Abstraction;

public interface IRequestHandler<TQuery, TResponse>
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}