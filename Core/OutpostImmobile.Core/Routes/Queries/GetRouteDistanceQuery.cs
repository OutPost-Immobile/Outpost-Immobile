using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;

namespace OutpostImmobile.Core.Routes.Queries;

public record GetRouteDistanceQuery : IRequest<GetRouteDistanceQuery, Task<long>>
{
    public required long RouteId { get; init; }
}

internal class GetRouteDistanceQueryHandler : IRequestHandler<GetRouteDistanceQuery, Task<long>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetRouteDistanceQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<long> Handle(GetRouteDistanceQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var route = await context.Routes
            .Where(x => x.Id == request.RouteId)
            .Select(x => new
            {
                x.StartAddressId,
                x.EndAddressId
            })
            .FirstAsync(ct);

        var startAddress = await context.Addresses
            .Where(x => x.Id == route.StartAddressId)
            .Select(x => x.Location)
            .FirstAsync(ct);
        
        var endAddress = await context.Addresses
            .Where(x => x.Id == route.EndAddressId)
            .Select(x => x.Location)
            .FirstAsync(ct);
        
        return await context.Database
            .SqlQuery<long>($"SELECT calculate_driving_distance({startAddress}, {endAddress})")
            .FirstAsync(ct);
    }
}