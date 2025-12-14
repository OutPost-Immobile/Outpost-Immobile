using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Common.Helpers;

namespace OutpostImmobile.Core.Common;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddScoped<IStaticEnumHelper, StaticEnumHelper>();
        
        return services;
    }
}