using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Mediator.Internal;

namespace OutpostImmobile.Core.Mediator;

public static class ServiceExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [ Assembly.GetExecutingAssembly()];
        }
        
        var registry = new HandlerRegistry();

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(x => x is { IsAbstract: false, IsInterface: false })
                .Where(x => x.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                services.AddTransient(handlerType);
                
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
                
                var requestType = interfaceType.GetGenericArguments()[0];
                
                registry.Register(requestType, handlerType);
            }
        }
        
        services.AddSingleton(registry);
        services.AddSingleton<IMediator, Internal.Mediator>();

        return services;
    }
}