namespace OutpostImmobile.Core.Mediator.Internal;

public class HandlerRegistry
{
    private Dictionary<Type, Type> _handlerTypes = new();
    
    public void Register(Type requestType, Type handlerType)
    {
        if (!_handlerTypes.TryAdd(requestType, handlerType))
        {
            throw new InvalidOperationException($"Handler for {requestType.Name} is already registered.");
        }
    }

    public Type GetHandlerType(Type requestType)
    {
        return _handlerTypes.TryGetValue(requestType, out var handlerType)
            ? handlerType 
            : throw new InvalidOperationException($"No handler registered for {requestType.Name}");
    }
}