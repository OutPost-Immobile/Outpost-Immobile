using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Tests.Acceptance.PU16;

/// <summary>
/// Decision Table: Wydawanie paczek (PU16)
/// Używa IParcelRepository i IMaczkopatRepository
/// </summary>
public class ParcelRelease
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IMaczkopatRepository _maczkopatRepository;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _lockerOpened;
    private bool _statusChanged;
    private string _errorMessage = string.Empty;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;

    public ParcelRelease()
    {
        _parcelRepository = TestDbContextFactory.GetService<IParcelRepository>();
        _maczkopatRepository = TestDbContextFactory.GetService<IMaczkopatRepository>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _lockerOpened = false;
        _statusChanged = false;
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

        var parcel = maczkopat.Parcels
            .FirstOrDefault(p => p.FriendlyId == ParcelFriendlyId && p.Status == ParcelStatus.InMaczkopat);

        // Scenariusz 2: Brak paczki w maczkopacie
        if (parcel == null)
        {
            _errorMessage = "Brak paczki w maczkopacie";
            _lockerOpened = false;
            _statusChanged = false;
            return;
        }

        try
        {
            // 1.1 Skrytka się otwiera
            await _maczkopatRepository.AddLogAsync(
                maczkopat.Id, 
                MaczkopatEventLogType.LockerOpened, 
                CancellationToken.None);
            
            _lockerOpened = true;

            // 1.3 Zmiana statusu paczki
            await _parcelRepository.UpdateParcelStatusAsync(
                ParcelFriendlyId, 
                ParcelStatus.Delivered, 
                "Odebrana");
            
            _statusChanged = true;
        }
        catch (MaczkopatStateException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (EntityNotFoundException ex)
        {
            _errorMessage = ex.Message;
        }
    }

    public bool LockerOpened() => _lockerOpened;
    public bool StatusChanged() => _statusChanged;
    public string ErrorMessage() => _errorMessage;

    public string FinalStatus()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var parcel = context.Parcels.FirstOrDefault(p => p.FriendlyId == ParcelFriendlyId);
        return parcel?.Status.ToString() ?? "Nieznaleziona";
    }

    public int ParcelCountAfterRelease()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);
        
        return maczkopat?.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat) ?? -1;
    }
}

/// <summary>
/// Decision Table: Próba wydania nieistniejącej paczki (PU16)
/// </summary>
public class ReleaseNonExistingParcel
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;

    public ReleaseNonExistingParcel()
    {
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public bool ParcelExistsInMaczkopat()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var maczkopat = context.Maczkopats
            .Include(m => m.Parcels)
            .FirstOrDefault(m => m.Code == MaczkopatCode);

        return maczkopat?.Parcels
            .Any(p => p.FriendlyId == ParcelFriendlyId && p.Status == ParcelStatus.InMaczkopat) ?? false;
    }

    public string ErrorMessage()
    {
        if (!ParcelExistsInMaczkopat())
        {
            return "Brak paczki w maczkopacie";
        }
        return string.Empty;
    }
}

/// <summary>
/// Setup Table: Przygotowanie danych dla PU16
/// </summary>
public class ParcelReleaseSetup
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string InitialStatus { get; set; } = "InMaczkopat";
    public string Product { get; set; } = "Standard";
    public long MaczkopatCapacity { get; set; } = 10;

    public ParcelReleaseSetup()
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
                Capacity = MaczkopatCapacity,
                AreaId = area.Id,
                AddressId = address.Id
            };
            context.Maczkopats.Add(maczkopat);
            context.SaveChanges();
        }

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
/// Teardown: Czyszczenie po testach PU16
/// </summary>
public class ParcelReleaseTeardown
{
    public void Execute()
    {
        TestDbContextFactory.Reset();
    }

    public bool Cleaned() => true;
}
