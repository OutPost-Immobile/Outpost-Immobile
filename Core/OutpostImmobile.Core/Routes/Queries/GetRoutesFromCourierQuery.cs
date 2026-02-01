using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Routes.QueryResult;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Routes.Queries;

public class GetRoutesFromCourierQuery : IRequest<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    public required Guid CourierId { get; init; }
}

internal class GetRoutesFromCourierQueryHandler : IRequestHandler<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetRoutesFromCourierQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<RouteDto>> Handle(GetRoutesFromCourierQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        var courierRouteId = await context.UsersInternal
            .Where(x => x.Id == request.CourierId)
            .Select(x => x.RouteId)
            .FirstOrDefaultAsync(ct) ?? null;

        if (courierRouteId == null)
        {
            throw new EntityNotFoundException("Route not found");
        }

        return await context.Routes
            .Where(x => x.Id == courierRouteId)
            .Select(x => new RouteDto
            {
                RouteId = x.Id,
                Distance = x.Distance,
                StartAddressName = x.StartAddressName,
                EndAddressName = x.EndAddressName
            })
            .ToListAsync(ct);
    }
}