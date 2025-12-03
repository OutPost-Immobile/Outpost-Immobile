using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string? connStr)
    {

        services.AddDbContext<OutpostImmobileDbContext>((options) => options
            .UseNpgsql(connStr, o => o.UseNetTopologySuite()));

        services
            .AddScoped<IEventLogFactory, MaczkopatEventLogFactory>()
            .AddScoped<IEventLogFactory, CommunicationsEventLogFactory>()
            .AddScoped<IEventLogFactory, ParcelEventLogFactory>()
            .AddScoped<IParcelRepository, ParcelRepository>()
            .AddScoped<IMaczkopatRepository, MaczkopatRepository>()
            .AddScoped<IRouteRepository, RouteRepository>()
            .AddScoped<ICommunicationEventLogRepository, CommunicationEventLogRepository>();
        
        return services;
    }
}