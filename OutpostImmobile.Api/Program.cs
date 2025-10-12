using System.Reflection;
using DispatchR.Extensions;
using OutpostImmobile.Api.Extensions;
using OutpostImmobile.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load(builder.Configuration["DispatchR:CoreAssembly"]));
});
builder.Services.AddSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddCoreServices();

var app = builder.Build();

//Tutaj mapujemy wszystkie endpointy
app.MapRoutes();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();