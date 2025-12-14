using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class ParcelRepository : IParcelRepository
{
    private readonly OutpostImmobileDbContext _context;
    private readonly IEventLogFactory _eventLogFactory;

    public ParcelRepository(OutpostImmobileDbContext context, ParcelEventLogFactory eventLogFactory)
    {
        _context = context;
        _eventLogFactory = eventLogFactory;
    }

    public Task UpdateParcelStatusAsync(Guid parcelId, ParcelStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<ParcelEntity> GetParcelsFromMaczkopatAsync(Guid maczkopatId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ParcelEventLogEntity>> GetParcelEventLogsAsync(string friendlyId)
    {
        var parcelId = await _context.Parcels
            .Where(p => p.FriendlyId == friendlyId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (parcelId == Guid.Empty)
        {
            throw new EntityNotFoundException("Parcel not found");
        }
        
        return await _context.ParcelEventLogs
            .AsNoTracking()
            .Where(x => x.ParcelId == parcelId)
            .ToListAsync();
    }
}