using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

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
        
        var vehicle = await context.Vehicles
            .AsNoTracking()
            .Where(x => x.DriverId == courierId)
            .FirstOrDefaultAsync();

        if (vehicle is null)
        {
            throw new EntityNotFoundException("Vehicle not found");
        }

        return await context.Routes
            .AsNoTracking()
            .Where(x => x.AssignedVehicles.Contains(vehicle))
            .ToListAsync();
    }
}