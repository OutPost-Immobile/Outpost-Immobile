using MailKit;
using Microsoft.Extensions.DependencyInjection;

namespace Communication;


public static class ServiceExtensions
{
    public static IServiceCollection AddCommunicationServices(this IServiceCollection services)
    {
        services.AddTransient<IMailService, MailService>();
        return services;
    }
}