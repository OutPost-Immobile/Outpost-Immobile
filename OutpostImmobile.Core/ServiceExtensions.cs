using Communication;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication;

namespace OutpostImmobile.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddCommunicationServices();
        return services;
    }
}