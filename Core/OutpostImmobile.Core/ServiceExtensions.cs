using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication;
using OutpostImmobile.Core.Common;
using OutpostImmobile.Core.Integrations.KMZB;
using OutpostImmobile.Core.Settings;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddCommunicationServices(configuration);
        services.AddKMZB();
        services.AddCommonServices();
        
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateOnStart()
            .ValidateDataAnnotations();

        services
            .AddScoped<UserManager<UserInternal>>()
            .AddScoped<SignInManager<UserInternal>>();
        
        return services;
    }
}