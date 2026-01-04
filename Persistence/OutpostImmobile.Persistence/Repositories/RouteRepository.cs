using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Persistence.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public RouteRepository(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var courierRouteId = await context.UsersInternal
            .Where(x => x.Id == courierId)
            .Select(x => (long?) x.RouteId)
            .FirstOrDefaultAsync() ?? null;

        if (courierRouteId == null)
        {
            throw new EntityNotFoundException("Route not found");
        }
        
        return await context.Routes
            .AsNoTracking()
            .Where(x => x.Id == courierRouteId)
            .ToListAsync();
    }

    public async Task<List<(bool, Point)>> GetPointsFromRouteAsync(long routeId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var route = await context.Routes
            .Where(x => x.Id == routeId)
            .Select(x => new
            {
                x.StartAddressId,
                x.EndAddressId
            })
            .FirstOrDefaultAsync();

        if (route is null)
        {
            throw new EntityNotFoundException($"Route with id {routeId} does not exist");
        }

        var startLocationPoint = await context.Addresses
            .Where(x => x.Id == route.StartAddressId)
            .Select(x => x.Location)
            .FirstAsync();
        
        var endLocationPoint = await context.Addresses
            .Where(x => x.Id == route.EndAddressId)
            .Select(x => x.Location)
            .FirstAsync();
        
        return [ (true, startLocationPoint), (false, endLocationPoint) ];
    }
    
    public async IAsyncEnumerable<RouteSegmentDto> GetRouteGeoJsonAsync(List<(bool, Point)> points)
    {
        if (points.Count > 2)
        {
            throw new WrongPointsCountException("Route has too many points");
        }
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        // Item1 - IsStart
        var start = points.First(x => x.Item1).Item2;
        var end = points.First(x => !x.Item1).Item2;
        
        var query = "SELECT * FROM get_hybrid_route({0}, {1})";
        
        var result = context.Database
            .SqlQueryRaw<RouteSegmentDto>(query, start, end
            )
            .AsAsyncEnumerable();

        await foreach (var segment in result)
        {
            yield return segment;
        }
    }
}