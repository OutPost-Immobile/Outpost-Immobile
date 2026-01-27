using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Api;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Core.Tests.Acceptance;

/// <summary>
/// Fabryka kontekstu aplikacji dla testów akceptacyjnych.
/// Konfiguruje WebApplicationFactory z InMemory database i mock serwisami.
/// </summary>
public static class TestDbContextFactory
{
    private static WebApplicationFactory<Program>? _factory;
    private static HttpClient? _httpClient;
    private static IServiceProvider? _serviceProvider;
    private static readonly object Lock = new();
    private static readonly string DatabaseName = $"OutpostImmobile_AcceptanceTests_{Guid.NewGuid()}";

    public static WebApplicationFactory<Program> GetFactory()
    {
        lock (Lock)
        {
            if (_factory != null) return _factory;

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    
                    builder.ConfigureServices(services =>
                    {
                        // Usuń istniejące rejestracje DbContext
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<OutpostImmobileDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        var factoryDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(IDbContextFactory<OutpostImmobileDbContext>));
                        if (factoryDescriptor != null)
                            services.Remove(factoryDescriptor);

                        // Dodaj InMemory database
                        services.AddDbContext<OutpostImmobileDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(DatabaseName);
                        });

                        services.AddSingleton<IDbContextFactory<OutpostImmobileDbContext>>(sp =>
                            new TestInMemoryDbContextFactory(DatabaseName));

                        // Podmień serwisy na mocki
                        var mailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMailService));
                        if (mailDescriptor != null)
                            services.Remove(mailDescriptor);
                        services.AddSingleton<IMailService, MockMailService>();

                        var kmzbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IKMZBService));
                        if (kmzbDescriptor != null)
                            services.Remove(kmzbDescriptor);
                        services.AddSingleton<IKMZBService, MockKMZBService>();
                    });
                });

            _serviceProvider = _factory.Services;
            return _factory;
        }
    }

    public static HttpClient GetHttpClient()
    {
        lock (Lock)
        {
            if (_httpClient != null) return _httpClient;
            
            _httpClient = GetFactory().CreateClient();
            return _httpClient;
        }
    }

    /// <summary>
    /// Pobiera HttpClient z autoryzacją dla określonej roli.
    /// </summary>
    public static async Task<HttpClient> GetAuthenticatedHttpClientAsync(string role = "Admin")
    {
        var client = GetFactory().CreateClient();
        
        // W testach możemy dodać token do nagłówków
        // Dla uproszczenia pomijamy autoryzację w środowisku testowym
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", $"test-token-{role}");
        
        return client;
    }

    public static IServiceProvider GetServiceProvider()
    {
        return GetFactory().Services;
    }

    public static OutpostImmobileDbContext GetContext()
    {
        var factory = GetServiceProvider().GetRequiredService<IDbContextFactory<OutpostImmobileDbContext>>();
        return factory.CreateDbContext();
    }

    public static T GetService<T>() where T : notnull
    {
        using var scope = GetServiceProvider().CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    public static void Reset()
    {
        lock (Lock)
        {
            _httpClient?.Dispose();
            _httpClient = null;
            
            _factory?.Dispose();
            _factory = null;
            
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

/// <summary>
/// Klasa pomocnicza do wykonywania żądań HTTP w testach
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> GetFromJsonAsync<T>(this HttpClient client, string requestUri)
    {
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, content);
    }
}