using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Mediator.Abstraction;

namespace OutpostImmobile.Core.Mediator.Internal;

public class Mediator : IMediator
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HandlerRegistry _handlerRegistry;

    public Mediator(IServiceScopeFactory scopeFactory, HandlerRegistry handlerRegistry)
    {
        _scopeFactory = scopeFactory;
        _handlerRegistry = handlerRegistry;
    }

    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var handlerType = _handlerRegistry.GetHandlerType(requestType);
        
        using var scope = _scopeFactory.CreateScope();
        var handlerInstance = scope.ServiceProvider.GetRequiredService(handlerType);
        
        var typedHandler = (IRequestHandler<TRequest, TResponse>)handlerInstance;
        
        return typedHandler.Handle((TRequest)request, ct);
    }
}