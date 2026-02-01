using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interceptors;

namespace OutpostImmobile.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string? connStr)
    {
        services.AddSingleton<AuditableEntityInterceptor>();
        
        services.AddDbContextFactory<OutpostImmobileDbContext>((sp, options) => options
            .UseNpgsql(connStr, o => o.UseNetTopologySuite())
            .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        services
            .AddScoped<MaczkopatEventLogFactory>()
            .AddScoped<CommunicationsEventLogFactory>()
            .AddScoped<ParcelEventLogFactory>()
            .AddKeyedScoped<IEventLogFactory, MaczkopatEventLogFactory>("Maczkopat")
            .AddKeyedScoped<IEventLogFactory, CommunicationsEventLogFactory>("Communications")
            .AddKeyedScoped<IEventLogFactory, ParcelEventLogFactory>("Parcel");
        
        services.AddIdentity<UserInternal, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<OutpostImmobileDbContext>()
            .AddDefaultTokenProviders();
        
        return services;
    }
}