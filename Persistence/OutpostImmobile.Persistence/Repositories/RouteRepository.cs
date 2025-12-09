using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly OutpostImmobileDbContext _context;

    public RouteRepository(OutpostImmobileDbContext context)
    {
        _context = context;
    }

    public Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId)
    {
        throw new NotImplementedException();
    }
}