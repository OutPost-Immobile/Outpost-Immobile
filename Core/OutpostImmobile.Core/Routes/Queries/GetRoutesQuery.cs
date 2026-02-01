using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Routes.QueryResult;
using OutpostImmobile.Persistence;

namespace OutpostImmobile.Core.Routes.Queries;

public record GetRoutesQuery : IRequest<GetRoutesQuery, Task<List<RouteDto>>>;

internal class GetRoutesQueryHandler : IRequestHandler<GetRoutesQuery, Task<List<RouteDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetRoutesQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<List<RouteDto>> Handle(GetRoutesQuery query, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        return await context.Routes
            .Select(x => new RouteDto
            {
                RouteId = x.Id,
                StartAddressName = x.StartAddressName,
                EndAddressName = x.EndAddressName,
                Distance = x.Distance
            })
            .ToListAsync(ct);
    }
}
