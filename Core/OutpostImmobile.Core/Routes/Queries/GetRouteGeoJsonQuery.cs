using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Core.Routes.Queries;

public record GetRouteGeoJsonQuery : IRequest<GetRouteGeoJsonQuery, IAsyncEnumerable<RouteSegmentDto>>
{
    public required long RouteId { get; init; }
}

internal class GetRouteGeoJsonQueryHandler : IRequestHandler<GetRouteGeoJsonQuery, IAsyncEnumerable<RouteSegmentDto>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetRouteGeoJsonQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async IAsyncEnumerable<RouteSegmentDto> Handle(GetRouteGeoJsonQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        context.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
        
        var route = await context.Routes
            .Where(x => x.Id == request.RouteId)
            .Select(x => new
            {
                x.StartAddressId,
                x.EndAddressId
            })
            .FirstOrDefaultAsync(ct);

        if (route is null)
        {
            throw new EntityNotFoundException($"Route with id {request.RouteId} does not exist");
        }

        var startLocationPoint = await context.Addresses
            .Where(x => x.Id == route.StartAddressId)
            .Select(x => x.Location)
            .FirstAsync(ct);
        
        var endLocationPoint = await context.Addresses
            .Where(x => x.Id == route.EndAddressId)
            .Select(x => x.Location)
            .FirstAsync(ct);
        
        var query = "SELECT * FROM get_hybrid_route({0}, {1})";
        
        var result = context.Database
            .SqlQueryRaw<RouteSegmentDto>(query, startLocationPoint, endLocationPoint)
            .AsAsyncEnumerable();

        await foreach (var segment in result)
        {
            yield return segment;
        }
    }
}