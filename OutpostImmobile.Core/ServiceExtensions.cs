using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication;
using OutpostImmobile.Core.Integrations.KMZB;

namespace OutpostImmobile.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddCommunicationServices();
        services.AddKMZB();
        
        return services;
    }
}