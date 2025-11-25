namespace OutpostImmobile.Core.Paralizator.Abstraction;

public interface IRequestHandler<TQuery, TResponse>
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}