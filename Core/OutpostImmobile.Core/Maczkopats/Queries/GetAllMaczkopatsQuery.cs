using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Maczkopats.QueryResults;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;

namespace OutpostImmobile.Core.Maczkopats.Queries;

public record GetAllMaczkopatsQuery : IRequest<GetAllMaczkopatsQuery, Task<List<MaczkopatDto>>>;

internal class GetAllMaczkopatsQueryHandler : IRequestHandler<GetAllMaczkopatsQuery, Task<List<MaczkopatDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetAllMaczkopatsQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<MaczkopatDto>> Handle(GetAllMaczkopatsQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        return await context.Maczkopats
            .Select(m => new MaczkopatDto
            {
                Id = m.Id,
                Code = m.Code,
                Capacity = m.Capacity,
                AreaName = m.Area.AreaName,
                City = m.Address.City,
                Street = m.Address.Street,
                BuildingNumber = m.Address.BuildingNumber,
                ParcelCount = m.Parcels.Count
            })
            .ToListAsync(ct);
    }
}
