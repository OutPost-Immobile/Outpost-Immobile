using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Tests.Acceptance.PU8;

/// <summary>
/// Decision Table: Zawożenie paczek do magazynu (PU8)
/// Używa IParcelRepository
/// </summary>
public class WarehouseTransport
{
    private readonly IParcelRepository _parcelRepository;
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    
    private bool _transportSuccess;
    private string _errorMessage = string.Empty;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string InitialStatus { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string CourierId { get; set; } = string.Empty;

    public WarehouseTransport()
    {
        _parcelRepository = TestDbContextFactory.GetService<IParcelRepository>();
        _dbContextFactory = TestDbContextFactory.GetService<IDbContextFactory<OutpostImmobileDbContext>>();
    }

    public void Reset()
    {
        _transportSuccess = false;
        _errorMessage = string.Empty;
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

        if (parcel == null)
        {
            _errorMessage = "Paczka nie istnieje";
            _transportSuccess = false;
            return;
        }

        // Sprawdź czy paczka jest nadana (Sent)
        if (parcel.Status != ParcelStatus.Sent)
        {
            _errorMessage = "Paczka nie jest oznaczona jako nadana";
            _transportSuccess = false;
            return;
        }

        try
        {
            // Użyj repozytorium do aktualizacji statusu
            await _parcelRepository.UpdateParcelStatusAsync(
                ParcelFriendlyId, 
                ParcelStatus.InWarehouse, 
                "W magazynie");

            _transportSuccess = true;
        }
        catch (MaczkopatStateException ex)
        {
            _errorMessage = ex.Message;
            _transportSuccess = false;
        }
        catch (EntityNotFoundException ex)
        {
            _errorMessage = ex.Message;
            _transportSuccess = false;
        }
    }

    public string FinalStatus()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var parcel = context.Parcels.FirstOrDefault(p => p.FriendlyId == ParcelFriendlyId);
        return parcel?.Status.ToString() ?? "Nieznaleziona";
    }

    public bool TransportSuccessful() => _transportSuccess;
    public string ErrorMessage() => _errorMessage;
}

/// <summary>
/// Setup Table: Przygotowanie danych dla PU8
/// </summary>
public class WarehouseTransportSetup
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public string ParcelFriendlyId { get; set; } = string.Empty;
    public string MaczkopatCode { get; set; } = string.Empty;
    public string InitialStatus { get; set; } = "Sent";
    public string Product { get; set; } = "Standard";

    public WarehouseTransportSetup()
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
                Capacity = 10,
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
/// Teardown: Czyszczenie po testach PU8
/// </summary>
public class WarehouseTransportTeardown
{
    public void Execute()
    {
        TestDbContextFactory.Reset();
    }

    public bool Cleaned() => true;
}
