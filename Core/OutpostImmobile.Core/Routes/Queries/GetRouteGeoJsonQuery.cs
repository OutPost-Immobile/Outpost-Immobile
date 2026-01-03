using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Core.Routes.Queries;

public record GetRouteGeoJsonQuery : IRequest<GetRouteGeoJsonQuery, Task<IAsyncEnumerable<RouteSegmentDto>>>
{
    public required long RouteId { get; init; }
}

internal class GetRouteGeoJsonQueryHandler : IRequestHandler<GetRouteGeoJsonQuery, Task<IAsyncEnumerable<RouteSegmentDto>>>
{
    private readonly IRouteRepository _routeRepository;

    public GetRouteGeoJsonQueryHandler(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<IAsyncEnumerable<RouteSegmentDto>> Handle(GetRouteGeoJsonQuery request, CancellationToken cancellationToken)
    {
        var points = await _routeRepository.GetPointsFromRouteAsync(request.RouteId);
        
        return await _routeRepository.GetRouteGeoJsonAsync(points);
    }
}