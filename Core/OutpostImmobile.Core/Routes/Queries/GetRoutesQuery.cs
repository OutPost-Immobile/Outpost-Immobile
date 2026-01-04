using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Routes.QueryResult;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Routes.Queries;

public record GetRoutesQuery : IRequest<GetRoutesQuery, Task<List<RouteDto>>>;

internal class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, Task<List<RouteDto>>>
{
    private readonly IRouteRepository _routeRepository;

    public GetRoutesQueryHandler(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<List<RouteDto>> Handle(GetRoutesQuery query, CancellationToken cancellationToken)
    {
        var routes = await _routeRepository.GetRoutesAsync();

        return routes.Select(x => new RouteDto
        {
            RouteId = x.Id,
            StartAddressName = x.StartAddressName,
            EndAddressName = x.EndAddressName,
            Distance = x.Distance
        }).ToList();
    }
}
