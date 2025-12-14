using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication;
using OutpostImmobile.Core.Common;
using OutpostImmobile.Core.Integrations.KMZB;

namespace OutpostImmobile.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddCommunicationServices(configuration);
        services.AddKMZB();
        services.AddCommonServices();
        return services;
    }
}