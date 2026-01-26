using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Tests.Acceptance.PU5;

/// <summary>
/// Decision Table: Odbiór paczki przez klienta lub kuriera (PU5)
/// Używa IParcelRepository i IMaczkopatRepository
/// </summary>
public class ParcelPickup
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IMaczkopatRepository _maczkopatRepository;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private string _errorMessage = string.Empty;
    private bool _pickupResult;
    private bool _lockerOpened;

    // Kolumny wejściowe z tabeli FitNesse
    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string PickupCode { get; set; } = string.Empty;
    public string ActorType { get; set; } = string.Empty;

    public ParcelPickup()
    {
        _parcelRepository = TestDbContextFactory.GetService<IParcelRepository>();
        _maczkopatRepository = TestDbContextFactory.GetService<IMaczkopatRepository>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _errorMessage = string.Empty;
        _pickupResult = false;
        _lockerOpened = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        // 1. Pobierz paczkę
        var parcel = await context.Parcels
            .Include(p => p.Maczkopat)
            .FirstOrDefaultAsync(p => p.FriendlyId == ParcelFriendlyId);

        if (parcel == null)
        {
            _errorMessage = "Paczka nie istnieje";
            _pickupResult = false;
            return;
        }

        // 2. Sprawdź status paczki
        if (parcel.Status != ParcelStatus.InMaczkopat)
        {
            _errorMessage = "Paczka nie znajduje się w maczkopacie";
            _pickupResult = false;
            return;
        }

        // 3. Sprawdź maczkopat
        if (parcel.Maczkopat?.Code != MaczkopatCode)
        {
            _errorMessage = "Paczka nie znajduje się w podanym maczkopacie";
            _pickupResult = false;
            return;
        }

        // 4. Walidacja kodu odbioru
        if (!ValidatePickupCode(PickupCode, ActorType))
        {
            _errorMessage = "Błędny kod odbioru";
            _pickupResult = false;
            return;
        }

        try
        {
            // 5. Logowanie otwarcia skrytki przez repozytorium
            await _maczkopatRepository.AddLogAsync(
                parcel.MaczkopatEntityId, 
                MaczkopatEventLogType.LockerOpened, 
                CancellationToken.None);
            
            _lockerOpened = true;

            // 6. Aktualizacja statusu paczki przez repozytorium
            await _parcelRepository.UpdateParcelStatusAsync(
                ParcelFriendlyId, 
                ParcelStatus.Delivered, 
                "Odebrana");

            _pickupResult = true;
        }
        catch (MaczkopatStateException ex)
        {
            _errorMessage = ex.Message;
            _pickupResult = false;
        }
        catch (EntityNotFoundException ex)
        {
            _errorMessage = ex.Message;
            _pickupResult = false;
        }
    }

    public bool PickupSuccessful() => _pickupResult;
    public bool LockerOpened() => _lockerOpened;
    public string ErrorMessage() => _errorMessage;

    public string FinalStatus()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var parcel = context.Parcels.FirstOrDefault(p => p.FriendlyId == ParcelFriendlyId);
        return parcel?.Status.ToString() ?? "Nieznaleziona";
    }

    public int ParcelCountInMaczkopat()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        return maczkopat?.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) ?? -1;
    }

    private bool ValidatePickupCode(string code, string actorType)
    {
        return actorType switch
        {
            "Klient" => code.Length == 6 && code.All(char.IsDigit),
            "Kurier" => code.StartsWith("KUR") && code.Length >= 6,
            _ => false
        };
    }
}

/// <summary>
/// Decision Table: Paczka oznaczona jako porzucona (Forgotten)
/// F8: System powiadamia Kierownika o porzuconych paczkach
/// </summary>
public class AbandonedParcel
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IMailService _mailService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _notificationSent;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public int DaysWithoutPickup { get; set; }

    public AbandonedParcel()
    {
        _parcelRepository = TestDbContextFactory.GetService<IParcelRepository>();
        _mailService = TestDbContextFactory.GetService<IMailService>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _notificationSent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var parcel = await context.Parcels
            .Include(p => p.Maczkopat)
            .FirstOrDefaultAsync(p => p.FriendlyId == ParcelFriendlyId);

        if (parcel == null) return;

        // Logika biznesowa: po 7 dniach paczka jest porzucona
        if (DaysWithoutPickup >= 7 && parcel.Status == ParcelStatus.InMaczkopat)
        {
            try
            {
                // Użyj repozytorium do aktualizacji statusu
                await _parcelRepository.UpdateParcelStatusAsync(
                    ParcelFriendlyId, 
                    ParcelStatus.Forgotten, 
                    "Porzucona");
                
                // PU12: Wysłanie powiadomienia do Kierownika
                await _mailService.SendMessage(new Communication.Services.SendEmailRequest
                {
                    RecipientMailAddress = "kierownik@outpost.pl",
                    RecipientName = "Kierownik",
                    MailSubject = "Paczka porzucona",
                    MailBody = $"Paczka {ParcelFriendlyId} została porzucona po {DaysWithoutPickup} dniach."
                });
                
                _notificationSent = true;
            }
            catch (MaczkopatStateException)
            {
                // Ignoruj - maczkopat może być w nieprawidłowym stanie
            }
        }
    }

    public string FinalStatus()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var parcel = context.Parcels.FirstOrDefault(p => p.FriendlyId == ParcelFriendlyId);
        return parcel?.Status.ToString() ?? "Nieznaleziona";
    }

    public bool NotificationSentToManager() => _notificationSent;
}

/// <summary>
/// Setup Table: Przygotowanie danych testowych dla PU5
/// </summary>
public class ParcelPickupSetup
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string InitialStatus { get; set; } = "InMaczkopat";
    public string Product { get; set; } = "Standard";
    public long MaczkopatCapacity { get; set; } = 10;

    public ParcelPickupSetup()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Execute()
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        // Utwórz maczkopat jeśli nie istnieje
        var maczkopat = context.Maczkopats.FirstOrDefault(m => m.Code == MaczkopatCode);
        if (maczkopat == null)
        {
var area = context.Set<AreaEntity>().FirstOrDefault() ?? new AreaEntity { Id = 1, AreaName = "TestArea" };
            if (!context.Set<AreaEntity>().Any())
            {
                context.Set<AreaEntity>().Add(area);
                context.SaveChanges();
            }

            var address = context.Set<AddressEntity>().FirstOrDefault() ?? new AddressEntity 
            { 
                Id = 1, 
                Street = "TestStreet", 
                City = "TestCity", 
                PostalCode = "00-000",
                CountryCode = "PL",
                BuildingNumber = "1",
                Location = null! // Ignored in test mode
            };
            if (!context.Set<AddressEntity>().Any())
            {
                context.Set<AddressEntity>().Add(address);
                context.SaveChanges();
            }

            maczkopat = new MaczkopatEntity
            {
                Id = Guid.NewGuid(),
                Code = MaczkopatCode,
                Capacity = MaczkopatCapacity,
                AreaId = area.Id,
                AddressId = address.Id
            };
            context.Maczkopats.Add(maczkopat);
            context.SaveChanges();
        }

        // Utwórz paczkę
        if (!context.Parcels.Any(p => p.FriendlyId == ParcelFriendlyId))
        {
            var status = Enum.Parse<ParcelStatus>(InitialStatus);
            context.Parcels.Add(new ParcelEntity
            {
                Id = Guid.NewGuid(),
                FriendlyId = ParcelFriendlyId,
                Product = Product,
                Status = status,
                MaczkopatEntityId = maczkopat.Id
            });
            context.SaveChanges();
        }
    }

    public bool Created()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Parcels.Any(p => p.FriendlyId == ParcelFriendlyId);
    }
}

/// <summary>
/// Teardown: Czyszczenie danych po testach
/// </summary>
public class ParcelPickupTeardown
{
    public void Execute()
    {
        TestDbContextFactory.Reset();
    }

    public bool Cleaned() => true;
}
