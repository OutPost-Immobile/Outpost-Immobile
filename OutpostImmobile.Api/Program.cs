using System.Reflection;
using DispatchR.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Api.Extensions;
using OutpostImmobile.Core;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Interceptors;
using OutpostImmobile.Persistence.Seeding;
using OutpostImmobile.Communication.Options;
using Serilog;

namespace OutpostImmobile.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddOpenApi();
        
        builder.Services.AddDispatchR(options =>
        {
            options.Assemblies.Add(Assembly.Load(builder.Configuration["DispatchR:CoreAssembly"]));
        });
        
        builder.Services.AddSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(builder.Configuration));
        
        builder.Services
            .AddSingleton<AuditableEntityInterceptor>()
            .AddCoreServices();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", x =>
                {
                    x.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
        });
        
        var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<OutpostImmobileDbContext>((sp, options) => options
                .UseNpgsql(connStr, o => o.UseNetTopologySuite())
                .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));
        
        builder.Services.AddIdentity<UserInternal, IdentityRole<Guid>>(options =>
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
        
        builder.Services.Configure<MailOptions>(
            builder.Configuration.GetSection("MailOptions")
        );
        
        var app = builder.Build();
        
        app.MapRoutes();
        
        app.UseSerilogRequestLogging();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutpostImmobileDbContext>();
            await context.Database.MigrateAsync();
            
            await ApplicationSeeder.SeedAsync(context);
        }

        await app.RunAsync();
    }
}