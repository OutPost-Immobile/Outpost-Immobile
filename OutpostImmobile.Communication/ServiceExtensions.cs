using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;

namespace OutpostImmobile.Communication;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommunicationServices(this IServiceCollection services)
    {
        services.AddTransient<IMailService, MailService>();
        
        return services;
    }
}