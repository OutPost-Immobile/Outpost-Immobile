using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IRouteRepository
{
    Task<List<RouteEntity>> GetRoutesAsync();
    Task CalculateRouteDistanceAsync(long routeId);
    Task<List<RouteEntity>> GetRouteFromCourierAsync(Guid courierId);
    IAsyncEnumerable<RouteSegmentDto> GetRouteGeoJsonAsync(List<(bool, Point)> points);
    Task<List<(bool, Point)>> GetPointsFromRouteAsync(long routeId);
}