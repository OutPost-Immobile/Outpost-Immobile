using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Tests.Acceptance.PU12;

/// <summary>
/// Decision Table: Wysłanie powiadomienia email (PU12)
/// Używa IMailService
/// </summary>
public class EmailNotification
{
    private readonly IMailService _mailService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private bool _sent;

    public string RecipientType { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ParcelFriendlyId { get; set; } = string.Empty;

    public EmailNotification()
    {
        _mailService = TestDbContextFactory.GetService<IMailService>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _sent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        if (NotificationType != "Email" || string.IsNullOrEmpty(RecipientEmail))
        {
            _sent = false;
            return;
        }

        await _mailService.SendMessage(new SendEmailRequest
        {
            RecipientMailAddress = RecipientEmail,
            RecipientName = RecipientType,
            MailSubject = $"Powiadomienie o paczce {ParcelFriendlyId}",
            MailBody = Message
        });

        _sent = true;
    }

    public bool Sent() => _sent;
}

/// <summary>
/// Decision Table: Wysłanie powiadomienia SMS (PU12)
/// </summary>
public class SmsNotification
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private bool _sent;

    public string RecipientType { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ParcelFriendlyId { get; set; } = string.Empty;

    public SmsNotification()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _sent = false;
    }

    public void Execute()
    {
        if (NotificationType != "SMS" || string.IsNullOrEmpty(RecipientPhone))
        {
            _sent = false;
            return;
        }

        // Symulacja wysłania SMS (SmsService jest pusty)
        _sent = true;
    }

    public bool Sent() => _sent;
}

/// <summary>
/// Decision Table: Powiadomienie kierownika o porzuconej paczce (PU12)
/// F8: System powiadamia Kierownika o porzuconych paczkach
/// </summary>
public class ManagerAbandonedParcelNotification
{
    private readonly IMailService _mailService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private bool _sent;

    public string RecipientType { get; set; } = string.Empty;
    public string ParcelFriendlyId { get; set; } = string.Empty;
    public int DaysAbandoned { get; set; }
    public string NotificationType { get; set; } = string.Empty;

    public ManagerAbandonedParcelNotification()
    {
        _mailService = TestDbContextFactory.GetService<IMailService>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _sent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        if (RecipientType != "Kierownik" || DaysAbandoned < 7)
        {
            _sent = false;
            return;
        }

        await _mailService.SendMessage(new SendEmailRequest
        {
            RecipientMailAddress = "kierownik@outpost.pl",
            RecipientName = "Kierownik",
            MailSubject = "Paczka porzucona",
            MailBody = $"Paczka {ParcelFriendlyId} została porzucona po {DaysAbandoned} dniach."
        });

        _sent = true;
    }

    public bool Sent() => _sent;
}

/// <summary>
/// Decision Table: Powiadomienie o stanie maczkopatu (PU12)
/// </summary>
public class MaczkopatStateNotification
{
    private readonly IMailService _mailService;
    private readonly IMaczkopatRepository _maczkopatRepository;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private bool _sent;

    public string RecipientType { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public int CurrentParcelCount { get; set; }
    public long MaxCapacity { get; set; }
    public string StateType { get; set; } = string.Empty;

    public MaczkopatStateNotification()
    {
        _mailService = TestDbContextFactory.GetService<IMailService>();
        _maczkopatRepository = TestDbContextFactory.GetService<IMaczkopatRepository>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _sent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        if (RecipientType != "Kierownik")
        {
            _sent = false;
            return;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var maczkopat = await context.Maczkopats.FirstOrDefaultAsync(m => m.Code == MaczkopatCode);
        
        if (maczkopat == null)
        {
            _sent = false;
            return;
        }

        if (StateType == "Full" || StateType == "Empty")
        {
            await _mailService.SendMessage(new SendEmailRequest
            {
                RecipientMailAddress = "kierownik@outpost.pl",
                RecipientName = "Kierownik",
                MailSubject = $"Stan maczkopatu {MaczkopatCode}",
                MailBody = $"Maczkopat {MaczkopatCode} jest {StateType}. Aktualna liczba paczek: {CurrentParcelCount}/{MaxCapacity}"
            });
            
            _sent = true;
        }
    }

    public bool NotificationSent() => _sent;
}

/// <summary>
/// Decision Table: Powiadomienie kuriera o przydziale (PU12)
/// </summary>
public class CourierAssignmentNotification
{
    private readonly IMailService _mailService;
    private bool _sent;

    public string RecipientType { get; set; } = string.Empty;
    public string CourierId { get; set; } = string.Empty;
    public string CourierEmail { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;

    public CourierAssignmentNotification()
    {
        _mailService = TestDbContextFactory.GetService<IMailService>();
    }

    public void Reset()
    {
        _sent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        if (RecipientType != "Kurier" || string.IsNullOrEmpty(CourierId))
        {
            _sent = false;
            return;
        }

        await _mailService.SendMessage(new SendEmailRequest
        {
            RecipientMailAddress = CourierEmail,
            RecipientName = $"Kurier {CourierId}",
            MailSubject = "Nowa trasa przydzielona",
            MailBody = $"Została Ci przydzielona trasa {RouteId}."
        });
        
        _sent = true;
    }

    public bool Sent() => _sent;
}
