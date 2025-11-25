using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Core.Integrations.KMZB.Services;

namespace OutpostImmobile.Core.Integrations.KMZB;

public static class ServiceExtensions
{
    public static IServiceCollection AddKMZB(this IServiceCollection services)
    {
        services.AddTransient<IKMZBService, KMZBService>();
        
        return services;
    }
}