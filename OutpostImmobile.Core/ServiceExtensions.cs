using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication;
using OutpostImmobile.Core.Interfaces;
using OutpostImmobile.Core.Services;

namespace OutpostImmobile.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddCommunicationServices();
        services.AddScoped<IInfrastructureService, InfrastructureService>();
        
        return services;
    }
}