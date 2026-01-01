using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Extensions;
using OutpostImmobile.Api.Helpers;
using OutpostImmobile.Api.Utils;
using OutpostImmobile.Core;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Seeding;
using Scalar.AspNetCore;
using Serilog;

namespace OutpostImmobile.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); 
        });
        
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

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        })
        .AddCookie("MyCookie", options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.LoginPath = "/api/Users/Login";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireRole(nameof(UserRoleName.Admin)));
            
            options.AddPolicy(PolicyNames.AdminManager, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireRole(nameof(UserRoleName.Admin), nameof(UserRoleName.Manager)));
            
            options.AddPolicy(PolicyNames.AdminManagerCourier, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireRole(nameof(UserRoleName.Admin), nameof(UserRoleName.Manager), nameof(UserRoleName.Courier)));
            
            options.AddPolicy(PolicyNames.ClientOnly, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireRole(nameof(UserRoleName.Client)));
        });
        
        builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        
        var app = builder.Build();
        
        app.MapRoutes();
        
        app.UseSerilogRequestLogging();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapGet("/", () => Results.Redirect("/scalar/v1"))
                .ExcludeFromDescription();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserInternal>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            
            var context = scope.ServiceProvider.GetRequiredService<OutpostImmobileDbContext>();
            await context.Database.MigrateAsync();
            
            await ApplicationSeeder.SeedAsync(context, userManager, roleManager);
        }

        await app.RunAsync();
    }
}