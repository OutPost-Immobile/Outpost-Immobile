using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Routes.QueryResult;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Routes.Queries;

public class GetRoutesFromCourierQuery : IRequest<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    public required Guid CourierId { get; init; }
}

internal class GetRoutesFromCourierQueryHandler : IRequestHandler<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    private readonly IRouteRepository _repository;

    public GetRoutesFromCourierQueryHandler(IRouteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RouteDto>> Handle(GetRoutesFromCourierQuery request, CancellationToken cancellationToken)
    {
        var routes = await _repository.GetRouteFromCourierAsync(request.CourierId);

        return routes
            .Select(x => new RouteDto
            {
                RouteId = x.Id,
                Distance = x.Distance,
                StartAddressName = x.StartAddressName,
                EndAddressName = x.EndAddressName
            })
            .ToList();
    }
}