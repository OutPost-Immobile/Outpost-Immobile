using OutpostImmobile.Core.Mediator.Abstraction;

namespace OutpostImmobile.Core.Mediator;

public interface IMediator
{
    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request, CancellationToken cancellationToken = default);
}