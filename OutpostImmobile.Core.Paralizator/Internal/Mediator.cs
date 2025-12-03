using OutpostImmobile.Core.Paralizator.Abstraction;

namespace OutpostImmobile.Core.Paralizator.Internal;

internal class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private HandlerRegistry _handlerRegistry;

    public Mediator(IServiceProvider serviceProvider, HandlerRegistry handlerRegistry)
    {
        _serviceProvider = serviceProvider;
        _handlerRegistry = handlerRegistry;
    }

    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
    {
        throw new NotImplementedException();
    }
}