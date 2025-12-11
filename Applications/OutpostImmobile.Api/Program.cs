using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Api.Extensions;
using OutpostImmobile.Core;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Seeding;
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