using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Persistence.BusinessRules;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class ParcelRepository : IParcelRepository
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IEventLogFactory _eventLogFactory;

    public ParcelRepository([FromKeyedServices("Parcel")] IEventLogFactory eventLogFactory, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _eventLogFactory = eventLogFactory;
        _dbContextFactory = dbContextFactory;
    }

    public async Task UpdateParcelStatusAsync(string friendlyId, ParcelStatus status, string statusStr)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
         var parcelToUpdate = await context.Parcels
            .Include(x => x.ParcelEventLogs)
            .FirstOrDefaultAsync(p => p.FriendlyId == friendlyId);

        if (parcelToUpdate == null)
        {
            throw new EntityNotFoundException("Parcel not found");
        }

        var shouldUpdateParcelStatus =
            await MaczkopatBusinessRules.ShouldUpdateMaczkopatState(context, parcelToUpdate, status);

        if (!shouldUpdateParcelStatus)
        {
            throw new MaczkopatStateException("Could not update parcel status maczkopat is either full or not working");
        }

        var request = new CreateParcelEventLogTypeRequest
        {
            ParcelId = parcelToUpdate.Id,
            EventLog = ParcelEventLogType.StatusChange,
            Message = $"Status zmieniony na: {statusStr}"
        };

        var parcelEventLog = (ParcelEventLogEntity)(await _eventLogFactory.CreateEventLogAsync(request));
        
        parcelToUpdate.Status = status;
        parcelToUpdate.ParcelEventLogs.Add(parcelEventLog);
        
        await context.SaveChangesAsync();
    }

    public async Task<List<ParcelEntity>> GetParcelsFromMaczkopatAsync(Guid maczkopatId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Parcels
            .AsNoTracking()
            .Where(p => p.MaczkopatEntityId == maczkopatId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParcelEventLogEntity>> GetParcelEventLogsAsync(string friendlyId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var parcelId = await context.Parcels
            .Where(p => p.FriendlyId == friendlyId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (parcelId == Guid.Empty)
        {
            throw new EntityNotFoundException("Parcel not found");
        }
        
        return await context.ParcelEventLogs
            .AsNoTracking()
            .Where(x => x.ParcelId == parcelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tuple<string, Guid?>>> GetReceiverIdsFromParcels(IEnumerable<string> friendlyIds)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Parcels
            .Where(x => friendlyIds.Contains(x.FriendlyId))
            .Select(x => new Tuple<string, Guid?>(x.FriendlyId, x.ReceiverUserExternalId))
            .ToListAsync();
    }
}