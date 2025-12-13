using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Routes.QueryResult;

namespace OutpostImmobile.Core.Routes.Queries;

public class GetRoutesFromCourierQuery : IRequest<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    public required Guid CourierId { get; init; }
}

internal class GetRoutesFromCourierQueryHandler : IRequestHandler<GetRoutesFromCourierQuery, Task<List<RouteDto>>>
{
    public Task<List<RouteDto>> Handle(GetRoutesFromCourierQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}