using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Core.Integrations.KMZB.Services;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Acceptance;

/// <summary>
/// Fabryka kontekstu bazy danych i serwisów dla testów akceptacyjnych.
/// Konfiguruje DI container z wszystkimi zależnościami.
/// </summary>
public static class TestDbContextFactory
{
    private static IServiceProvider? _serviceProvider;
    private static readonly object Lock = new();
    private static readonly string DatabaseName = $"OutpostImmobile_AcceptanceTests_{Guid.NewGuid()}";

    public static IServiceProvider GetServiceProvider()
    {
        lock (Lock)
        {
            if (_serviceProvider != null) return _serviceProvider;

            var services = new ServiceCollection();

            // Rejestracja custom DbContextFactory dla testów
            services.AddSingleton<IDbContextFactory<OutpostImmobileDbContext>>(sp => 
                new TestInMemoryDbContextFactory(DatabaseName));

            // Rejestracja Factories (keyed services)
            services.AddKeyedSingleton<IEventLogFactory, MaczkopatEventLogFactory>("Maczkopat");
            services.AddKeyedSingleton<IEventLogFactory, ParcelEventLogFactory>("Parcel");
            services.AddSingleton<CommunicationsEventLogFactory>();

            // Rejestracja Repositories
            services.AddScoped<IParcelRepository, ParcelRepository>();
            services.AddScoped<IMaczkopatRepository, MaczkopatRepository>();
            services.AddScoped<ICommunicationEventLogRepository, CommunicationEventLogRepository>();

            // Rejestracja Services (mocki dla testów)
            services.AddScoped<IMailService, MockMailService>();
            services.AddScoped<IKMZBService, MockKMZBService>();

            _serviceProvider = services.BuildServiceProvider();
            return _serviceProvider;
        }
    }

    public static OutpostImmobileDbContext GetContext()
    {
        var factory = GetServiceProvider().GetRequiredService<IDbContextFactory<OutpostImmobileDbContext>>();
        return factory.CreateDbContext();
    }

    public static T GetService<T>() where T : notnull
    {
        return GetServiceProvider().GetRequiredService<T>();
    }

    public static void Reset()
    {
        lock (Lock)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _serviceProvider = null;
        }
    }
}

/// <summary>
/// Custom DbContextFactory dla testów - ustawia IsInTestEnv przed utworzeniem modelu
/// </summary>
public class TestInMemoryDbContextFactory : IDbContextFactory<OutpostImmobileDbContext>
{
    private readonly string _databaseName;
    private readonly DbContextOptions<OutpostImmobileDbContext> _options;

    public TestInMemoryDbContextFactory(string databaseName)
    {
        _databaseName = databaseName;
        _options = new DbContextOptionsBuilder<OutpostImmobileDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;
    }

    public OutpostImmobileDbContext CreateDbContext()
    {
        var context = new OutpostImmobileDbContext(_options)
        {
            IsInTestEnv = true
        };
        return context;
    }
}

/// <summary>
/// Mock serwisu mailowego dla testów
/// </summary>
public class MockMailService : IMailService
{
    public List<SendEmailRequest> SentEmails { get; } = new();
    
    public Task SendMessage(SendEmailRequest request)
    {
        SentEmails.Add(request);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Mock serwisu KMZB dla testów
/// </summary>
public class MockKMZBService : IKMZBService
{
    public int WarningsCreated { get; private set; }
    
    public Task CreateNewWarningAsync()
    {
        WarningsCreated++;
        return Task.CompletedTask;
    }
}