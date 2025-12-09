using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Mediator.Abstraction;

namespace OutpostImmobile.Core.Mediator.Internal;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private HandlerRegistry _handlerRegistry;

    public Mediator(IServiceProvider serviceProvider, HandlerRegistry handlerRegistry)
    {
        _serviceProvider = serviceProvider;
        _handlerRegistry = handlerRegistry;
    }

    public async Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var handlerType = _handlerRegistry.GetHandlerType(requestType);
        
        var handlerInstance = _serviceProvider.GetRequiredService(handlerType);
        
        var typedHandler = (IRequestHandler<TRequest, TResponse>)handlerInstance;
        
        return await typedHandler.Handle((TRequest)request, ct);
    }
}