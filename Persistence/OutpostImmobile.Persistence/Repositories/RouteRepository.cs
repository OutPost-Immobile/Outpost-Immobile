using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly OutpostImmobileDbContext _context;

    public RouteRepository(OutpostImmobileDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId)
    {
        var vehicle = await _context.Vehicles
            .AsNoTracking()
            .Where(x => x.DriverId == courierId)
            .FirstOrDefaultAsync();

        if (vehicle is null)
        {
            throw new EntityNotFoundException("Vehicle not found");
        }

        return await _context.Routes
            .AsNoTracking()
            .Where(x => x.AssignedVehicles.Contains(vehicle))
            .ToListAsync();
    }
}