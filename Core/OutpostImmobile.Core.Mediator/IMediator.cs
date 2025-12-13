using OutpostImmobile.Core.Mediator.Abstraction;

namespace OutpostImmobile.Core.Mediator;

public interface IMediator
{
    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request, CancellationToken cancellationToken = default);
}