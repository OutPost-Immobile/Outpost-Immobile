using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Options;
using OutpostImmobile.Communication.Services;

namespace OutpostImmobile.Communication;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommunicationServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptions<MailOptions>()
            .Bind(configuration.GetSection("MailOptions"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddTransient<IMailService, MailService>();
        
        return services;
    }
}