using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Parcels.Queries;

public record TrackParcelByFriendlyIdQuery : IRequest<TrackParcelByFriendlyIdQuery, Task<IEnumerable<ParcelLogDto>>>
{
    public required string FriendlyId { get; init; }
}

internal class TrackParcelByFriendlyIdQueryHandler : IRequestHandler<TrackParcelByFriendlyIdQuery, Task<IEnumerable<ParcelLogDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public TrackParcelByFriendlyIdQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<ParcelLogDto>> Handle(TrackParcelByFriendlyIdQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        var parcelId = await context.Parcels
            .Where(p => p.FriendlyId == request.FriendlyId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(ct);

        if (parcelId == Guid.Empty)
        {
            throw new EntityNotFoundException("Parcel not found");
        }
        
        return await context.ParcelEventLogs
            .Where(x => x.ParcelId == parcelId)
            .Select(x => new ParcelLogDto
            {
                Message = x.Message,
                CreatedAt = x.CreatedAt,
                ParcelEventLogType = x.ParcelEventLogType.ToString(),
                ParcelId = x.ParcelId,
            })
            .ToListAsync(ct);
    }

}
