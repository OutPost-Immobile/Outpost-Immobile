using OutpostImmobile.Persistence.Domain.Routes;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IRouteRepository
{
    Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId);
}