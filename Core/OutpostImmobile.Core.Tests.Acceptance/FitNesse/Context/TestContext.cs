using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Parcels.Commands;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Context;

/// <summary>
/// Shared test context for FitNesse acceptance tests.
/// Manages the in-memory database, services, and mediator for test execution.a
/// </summary>
public class TestContext : IDisposable
{
    private static TestContext? _instance;
    private static readonly object Lock = new();
    
    public IServiceProvider ServiceProvider { get; }
    public IDbContextFactory<OutpostImmobileDbContext> DbContextFactory { get; }
    public IMediator Mediator { get; }
    
    // Track sent emails for verification
    public List<SendEmailRequest> SentEmails { get; } = new();
    
    // Store current test state
    public Guid? CurrentMaczkopatId { get; set; }
    public Guid? CurrentUserId { get; set; }
    public string? CurrentUserRole { get; set; }
    public bool IsAuthenticated { get; set; }
    
    private TestContext()
    {
        var services = new ServiceCollection();
        
        // Configure in-memory database
        var dbName = $"OutpostImmobileTest_{Guid.NewGuid()}";
        services.AddDbContextFactory<OutpostImmobileDbContext>(options =>
        {
            options.UseInMemoryDatabase(dbName);
            options.EnableSensitiveDataLogging();
        });
        
        // Configure mocks
        var mailServiceMock = new Mock<IMailService>();
        mailServiceMock
            .Setup(x => x.SendMessage(It.IsAny<SendEmailRequest>()))
            .Callback<SendEmailRequest>(request => SentEmails.Add(request))
            .Returns(Task.CompletedTask);
        
        var kmzbServiceMock = new Mock<IKMZBService>();
        kmzbServiceMock
            .Setup(x => x.CreateNewWarningAsync())
            .Returns(Task.CompletedTask);
        
        services.AddSingleton(mailServiceMock.Object);
        services.AddSingleton(kmzbServiceMock.Object);
        
        // Register repositories
        services.AddScoped<IParcelRepository, ParcelRepository>();
        services.AddScoped<IMaczkopatRepository, MaczkopatRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register helpers
        services.AddScoped<IStaticEnumHelper, StaticEnumHelper>();
        
        // Register event log factories
        services.AddKeyedScoped<IEventLogFactory, ParcelEventLogFactory>("Parcel");
        services.AddKeyedScoped<IEventLogFactory, MaczkopatEventLogFactory>("Maczkopat");
        
        // Register mediator with command/query handlers
        services.AddMediator(typeof(BulkUpdateParcelStatusCommand).Assembly);
        
        ServiceProvider = services.BuildServiceProvider();
        DbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<OutpostImmobileDbContext>>();
        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        
        // Initialize database with test data
        InitializeDatabase().GetAwaiter().GetResult();
    }
    
    public static TestContext Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance ??= new TestContext();
                }
            }
            return _instance;
        }
    }
    
    public static void Reset()
    {
        lock (Lock)
        {
            _instance?.Dispose();
            _instance = null;
        }
    }
    
    private async Task InitializeDatabase()
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();
        context.IsInTestEnv = true;
        
        // Seed static enums for ParcelStatus translations
        var parcelStatusEnum = new StaticEnumEntity
        {
            EnumName = nameof(ParcelStatus),
            Translations = new List<StaticEnumTranslationEntity>
            {
                new() { EnumValue = (int)ParcelStatus.Expedited, EnumName = nameof(ParcelStatus), Translation = "Wysłana", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.Delivered, EnumName = nameof(ParcelStatus), Translation = "Dostarczona", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.InTransit, EnumName = nameof(ParcelStatus), Translation = "W tranzycie", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.InWarehouse, EnumName = nameof(ParcelStatus), Translation = "W magazynie", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.Forgotten, EnumName = nameof(ParcelStatus), Translation = "Zapomniana", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.Deleted, EnumName = nameof(ParcelStatus), Translation = "Usunięta", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.Sent, EnumName = nameof(ParcelStatus), Translation = "Wysłana", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.ToReturn, EnumName = nameof(ParcelStatus), Translation = "Do zwrotu", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.SendToStorage, EnumName = nameof(ParcelStatus), Translation = "Wysłana do magazynu", TranslationLanguage = TranslationLanguage.Pl },
                new() { EnumValue = (int)ParcelStatus.InMaczkopat, EnumName = nameof(ParcelStatus), Translation = "W maczkopacie", TranslationLanguage = TranslationLanguage.Pl }
            }
        };
        
        context.StaticEnums.Add(parcelStatusEnum);
        await context.SaveChangesAsync();
    }
    
    public async Task<OutpostImmobileDbContext> CreateContextAsync()
    {
        var context = await DbContextFactory.CreateDbContextAsync();
        context.IsInTestEnv = true;
        return context;
    }
    
    public void ClearSentEmails()
    {
        SentEmails.Clear();
    }
    
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
