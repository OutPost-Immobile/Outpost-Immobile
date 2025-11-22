using Microsoft.Extensions.DependencyInjection;

namespace OutpostImmobile.Core.Paralizator;

public static class ServiceExtensions
{
    public static IServiceCollection AddParalizator(this IServiceCollection services)
    {
        return services;
    }
}