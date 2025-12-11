using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interceptors;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string? connStr)
    {
        services.AddSingleton<AuditableEntityInterceptor>();
        
        services.AddDbContext<OutpostImmobileDbContext>((sp, options) => options
            .UseNpgsql(connStr, o => o.UseNetTopologySuite())
            .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));
        
        services
            .AddScoped<MaczkopatEventLogFactory>()
            .AddScoped<CommunicationsEventLogFactory>()
            .AddScoped<ParcelEventLogFactory>()
            .AddScoped<IEventLogFactory, MaczkopatEventLogFactory>()
            .AddScoped<IEventLogFactory, CommunicationsEventLogFactory>()
            .AddScoped<IEventLogFactory, ParcelEventLogFactory>()
            .AddScoped<IParcelRepository, ParcelRepository>()
            .AddScoped<IMaczkopatRepository, MaczkopatRepository>()
            .AddScoped<IRouteRepository, RouteRepository>()
            .AddScoped<ICommunicationEventLogRepository, CommunicationEventLogRepository>();
        
        services.AddIdentity<UserInternal, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<OutpostImmobileDbContext>()
            .AddDefaultTokenProviders();
        
        return services;
    }
}