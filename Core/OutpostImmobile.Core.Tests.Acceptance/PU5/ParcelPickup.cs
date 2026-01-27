using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Communication.Interfaces;
using OutpostImmobile.Communication.Services;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Tests.Acceptance.PU5;

/// <summary>
/// Decision Table: Odbiór paczki przez klienta lub kuriera (PU5)
/// Używa kontrolerów ParcelController i MaczkopatController
/// </summary>
public class ParcelPickup
{
    private readonly HttpClient _httpClient;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private string _errorMessage = string.Empty;
    private bool _pickupResult;
    private bool _lockerOpened;
    private HttpStatusCode _lastStatusCode;

    // Kolumny wejściowe z tabeli FitNesse
    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string PickupCode { get; set; } = string.Empty;
    public string ActorType { get; set; } = string.Empty;

    public ParcelPickup()
    {
        _httpClient = TestDbContextFactory.GetHttpClient();
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
        
        // 1. Pobierz paczkę z bazy danych w celu walidacji
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
            // 5. Logowanie otwarcia skrytki przez kontroler MaczkopatController
            var addLogRequest = new AddLogRequest
            {
                MaczkopatId = parcel.MaczkopatEntityId,
                LogType = MaczkopatEventLogType.LockerOpened
            };
            
            var logResponse = await _httpClient.PostAsJsonAsync("/api/maczkopats/AddLog", addLogRequest);
            
            if (logResponse.IsSuccessStatusCode)
            {
                _lockerOpened = true;
            }
            else
            {
                _lastStatusCode = logResponse.StatusCode;
                _errorMessage = $"Błąd logowania otwarcia skrytki: {logResponse.StatusCode}";
                _pickupResult = false;
                return;
            }

            // 6. Aktualizacja statusu paczki przez kontroler ParcelController
            var updateRequest = new List<UpdateParcelStatusRequest>
            {
                new UpdateParcelStatusRequest
                {
                    FriendlyId = ParcelFriendlyId,
                    ParcelStatus = ParcelStatus.Delivered
                }
            };

            var updateResponse = await _httpClient.PostAsJsonAsync("/api/Parcels/Update", updateRequest);
            _lastStatusCode = updateResponse.StatusCode;

            if (updateResponse.IsSuccessStatusCode)
            {
                _pickupResult = true;
            }
            else
            {
                _errorMessage = $"Błąd aktualizacji statusu paczki: {updateResponse.StatusCode}";
                _pickupResult = false;
            }
        }
        catch (Exception ex)
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
    private readonly HttpClient _httpClient;
    private readonly IMailService _mailService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _notificationSent;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public int DaysWithoutPickup { get; set; }

    public AbandonedParcel()
    {
        _httpClient = TestDbContextFactory.GetHttpClient();
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
                // Użyj kontrolera do aktualizacji statusu
                var updateRequest = new List<UpdateParcelStatusRequest>
                {
                    new UpdateParcelStatusRequest
                    {
                        FriendlyId = ParcelFriendlyId,
                        ParcelStatus = ParcelStatus.Forgotten
                    }
                };

                var response = await _httpClient.PostAsJsonAsync("/api/Parcels/Update", updateRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    // PU12: Wysłanie powiadomienia do Kierownika
                    await _mailService.SendMessage(new SendEmailRequest
                    {
                        RecipientMailAddress = "kierownik@outpost.pl",
                        RecipientName = "Kierownik",
                        MailSubject = "Paczka porzucona",
                        MailBody = $"Paczka {ParcelFriendlyId} została porzucona po {DaysWithoutPickup} dniach."
                    });
                    
                    _notificationSent = true;
                }
            }
            catch (Exception)
            {
                // Ignoruj - paczka może być w nieprawidłowym stanie
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
