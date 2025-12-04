namespace OutpostImmobile.Core.Paralizator.Internal;

public class HandlerRegistry
{
    private Dictionary<Type, Type> _handlerTypes = new();
    
    public void Register(Type requestType, Type handlerType)
    {
        _handlerTypes.Add(requestType, handlerType);
    }

    public Type GetHandlerType(Type requestType)
    {
        return _handlerTypes[requestType];
    }
}