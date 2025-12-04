using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IRouteRepository
{
    Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId);
}