using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Api.Extensions;
using OutpostImmobile.Core;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Seeding;
using Scalar.AspNetCore;
using Serilog;

namespace OutpostImmobile.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddOpenApi();
        
        builder.Services.AddSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(builder.Configuration));

        builder.Services.AddEndpointsApiExplorer();
        
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

        builder.Services
            .AddMediator(typeof(Core.ServiceExtensions).Assembly)
            .AddCoreServices(builder.Configuration)
            .AddPersistence(connStr);

        builder.Services.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<OutpostImmobileDbContext>();
        
        var app = builder.Build();
        
        app.MapRoutes();
        
        app.UseSerilogRequestLogging();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutpostImmobileDbContext>();
            await context.Database.MigrateAsync();
            
            await ApplicationSeeder.SeedAsync(context);
        }

        await app.RunAsync();
    }
}