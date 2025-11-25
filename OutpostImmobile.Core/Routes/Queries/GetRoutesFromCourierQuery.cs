using DispatchR.Abstractions.Send;
using OutpostImmobile.Core.Routes.QueryResult;

namespace OutpostImmobile.Core.Routes.Queries;

public class GetRoutesFromCourierQuery : IRequest<RouteDto, Task>
{
    public required Guid CourierId { get; init; }
}

internal class GetRoutesFromCourierQueryHandler : IRequestHandler<GetRoutesFromCourierQuery, Task<RouteDto>>
{
    public Task<RouteDto> Handle(GetRoutesFromCourierQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}