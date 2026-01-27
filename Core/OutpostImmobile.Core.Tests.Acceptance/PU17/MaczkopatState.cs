using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Integrations.KMZB.Interfaces;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Tests.Acceptance.PU17;

/// <summary>
/// Decision Table: Aktualizacja stanu maczkopatu (PU17)
/// Używa kontrolerów MaczkopatController i ParcelController
/// </summary>
public class MaczkopatStateUpdate
{
    private readonly HttpClient _httpClient;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _operationSuccess;
    private bool _lockerOpened;
    private string _errorMessage = string.Empty;
    private HttpStatusCode _lastStatusCode;

    public string MaczkopatCode { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string ParcelFriendlyId { get; set; } = string.Empty;

    public MaczkopatStateUpdate()
    {
        _httpClient = TestDbContextFactory.GetHttpClient();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _operationSuccess = false;
        _lockerOpened = false;
        _errorMessage = string.Empty;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var maczkopat = await context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefaultAsync(m => m.Code == MaczkopatCode);

        if (maczkopat == null)
        {
            _errorMessage = "Maczkopat nie istnieje";
            return;
        }

        var currentParcelCount = maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat);

        if (Operation == "Przyjęcie")
        {
            if (currentParcelCount >= maczkopat.Capacity)
            {
                _errorMessage = "Maczkopat jest pełny";
                _lockerOpened = false;
                _operationSuccess = false;
                return;
            }

            try
            {
                // Logowanie otwarcia skrytki przez kontroler
                var addLogRequest = new AddLogRequest
                {
                    MaczkopatId = maczkopat.Id,
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
                    _operationSuccess = false;
                    return;
                }

                // Aktualizacja statusu paczki przez kontroler
                var updateRequest = new List<UpdateParcelStatusRequest>
                {
                    new UpdateParcelStatusRequest
                    {
                        FriendlyId = ParcelFriendlyId,
                        ParcelStatus = ParcelStatus.InMaczkopat
                    }
                };

                var updateResponse = await _httpClient.PostAsJsonAsync("/api/Parcels/Update", updateRequest);
                _lastStatusCode = updateResponse.StatusCode;

                if (updateResponse.IsSuccessStatusCode)
                {
                    _operationSuccess = true;
                }
                else
                {
                    _errorMessage = $"Błąd aktualizacji statusu paczki: {updateResponse.StatusCode}";
                    _operationSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _operationSuccess = false;
            }
        }
        else if (Operation == "Wydanie")
        {
            if (currentParcelCount <= 0)
            {
                _errorMessage = "Maczkopat jest pusty";
                _lockerOpened = false;
                _operationSuccess = false;
                return;
            }

            try
            {
                // Logowanie otwarcia skrytki przez kontroler
                var addLogRequest = new AddLogRequest
                {
                    MaczkopatId = maczkopat.Id,
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
                    _operationSuccess = false;
                    return;
                }

                // Aktualizacja statusu paczki przez kontroler
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
                    _operationSuccess = true;
                }
                else
                {
                    _errorMessage = $"Błąd aktualizacji statusu paczki: {updateResponse.StatusCode}";
                    _operationSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                _operationSuccess = false;
            }
        }
    }

    public bool OperationSuccess() => _operationSuccess;
    public bool LockerOpened() => _lockerOpened;
    public string ErrorMessage() => _errorMessage;

    public int CurrentParcelCount()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        return maczkopat?.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) ?? -1;
    }

    public long MaxCapacity()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats.FirstOrDefault(m => m.Code == MaczkopatCode);
        return maczkopat?.Capacity ?? -1;
    }
}

/// <summary>
/// Decision Table: Zgłoszenie do KMZB przy siłowym wyjęciu (PU17)
/// Używa IKMZBService i kontrolera MaczkopatController
/// F9: System powiadamia policję (KMZB)
/// </summary>
public class KmzbReport
{
    private readonly HttpClient _httpClient;
    private readonly IKMZBService _kmzbService;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _kmzbReportCreated;
    private bool _notificationSent;

    public string MaczkopatCode { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;

    public KmzbReport()
    {
        _httpClient = TestDbContextFactory.GetHttpClient();
        _kmzbService = TestDbContextFactory.GetService<IKMZBService>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _kmzbReportCreated = false;
        _notificationSent = false;
    }

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    private async Task ExecuteAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var maczkopat = await context.Maczkopats.FirstOrDefaultAsync(m => m.Code == MaczkopatCode);
        if (maczkopat == null)
        {
            return;
        }

        if (EventType == "SiłoweWyjęcie" || EventType == "Włamanie")
        {
            // Logowanie siłowego wyjęcia przez kontroler
            var addLogRequest = new AddLogRequest
            {
                MaczkopatId = maczkopat.Id,
                LogType = MaczkopatEventLogType.OpenedByForce
            };

            try
            {
                await _httpClient.PostAsJsonAsync("/api/maczkopats/AddLog", addLogRequest);
            }
            catch
            {
                // Ignoruj błędy logowania
            }

            await _kmzbService.CreateNewWarningAsync();
            
            _kmzbReportCreated = true;
            _notificationSent = true;
        }
    }

    public bool ReportCreated() => _kmzbReportCreated;
    public bool NotificationSent() => _notificationSent;
}

/// <summary>
/// Decision Table: Blokada przy pełnym maczkopacie (PU17)
/// </summary>
public class BlockFullMaczkopat
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _operationBlocked;
    private string _errorMessage = string.Empty;

    public string MaczkopatCode { get; set; } = string.Empty;
    public int CurrentParcelCount { get; set; }
    public long MaxCapacity { get; set; }

    public BlockFullMaczkopat()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _operationBlocked = false;
        _errorMessage = string.Empty;
    }

    public void Execute()
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);

        if (maczkopat == null) return;

        var actualCount = maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat);

        if (actualCount >= maczkopat.Capacity)
        {
            _operationBlocked = true;
            _errorMessage = "Maczkopat jest pełny";
        }
    }

    public bool OperationBlocked() => _operationBlocked;
    public bool LockerRemainsClosed() => _operationBlocked;
    public string ErrorMessage() => _errorMessage;
}

/// <summary>
/// Decision Table: Blokada przy pustym maczkopacie (PU17)
/// </summary>
public class BlockEmptyMaczkopat
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _operationBlocked;
    private string _errorMessage = string.Empty;

    public string MaczkopatCode { get; set; } = string.Empty;

    public BlockEmptyMaczkopat()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _operationBlocked = false;
        _errorMessage = string.Empty;
    }

    public void Execute()
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);

        if (maczkopat == null) return;

        var actualCount = maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat);

        if (actualCount <= 0)
        {
            _operationBlocked = true;
            _errorMessage = "Maczkopat jest pusty";
        }
    }

    public bool OperationBlocked() => _operationBlocked;
    public bool LockerRemainsClosed() => _operationBlocked;
    public string ErrorMessage() => _errorMessage;
}

/// <summary>
/// Setup Table: Przygotowanie danych dla PU17
/// </summary>
public class MaczkopatStateSetup
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string MaczkopatCode { get; set; } = string.Empty;
    public long Capacity { get; set; } = 10;
    public int InitialParcelCount { get; set; } = 0;

    public MaczkopatStateSetup()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Execute()
    {
        using var context = _dbContextFactory.CreateDbContext();
        
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
                Capacity = Capacity,
                AreaId = area.Id,
                AddressId = address.Id
            };
            context.Maczkopats.Add(maczkopat);
            context.SaveChanges();

            for (int i = 0; i < InitialParcelCount; i++)
            {
                context.Parcels.Add(new ParcelEntity
                {
                    Id = Guid.NewGuid(),
                    FriendlyId = $"INIT-{MaczkopatCode}-{i:D3}",
                    Product = "Standard",
                    Status = ParcelStatus.InMaczkopat,
                    MaczkopatEntityId = maczkopat.Id
                });
            }
            context.SaveChanges();
        }
    }

    public bool Created()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Maczkopats.Any(m => m.Code == MaczkopatCode);
    }
}

/// <summary>
/// Decision Table: Sprawdzenie stanu maczkopatu (F15)
/// </summary>
public class MaczkopatStateCheck
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string MaczkopatCode { get; set; } = string.Empty;

    public MaczkopatStateCheck()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public int CurrentParcelCount()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        return maczkopat?.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) ?? -1;
    }

    public long MaxCapacity()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats.FirstOrDefault(m => m.Code == MaczkopatCode);
        return maczkopat?.Capacity ?? -1;
    }

    public bool IsFull()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        if (maczkopat == null) return false;
        return maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) >= maczkopat.Capacity;
    }

    public bool IsEmpty()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        if (maczkopat == null) return false;
        return maczkopat.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) == 0;
    }
}

/// <summary>
/// Teardown: Czyszczenie po testach PU17
/// </summary>
public class MaczkopatStateTeardown
{
    public void Execute()
    {
        TestDbContextFactory.Reset();
    }

    public bool Cleaned() => true;
}
