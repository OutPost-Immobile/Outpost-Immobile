using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Routes.Commands;

public record SaveCalculatedRouteDistanceCommand : IRequest<SaveCalculatedRouteDistanceCommand, Task>
{
    public required long RouteId { get; init; }
    public required long CalculatedDistance { get; init; }
}

internal class SaveCalculatedRouteDistanceCommandHandler : IRequestHandler<SaveCalculatedRouteDistanceCommand, Task>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public SaveCalculatedRouteDistanceCommandHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task Handle(SaveCalculatedRouteDistanceCommand request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        var route = await context.Routes
            .FirstOrDefaultAsync(x =>  x.Id == request.RouteId, ct);

        if (route is null)
        {
            throw new EntityNotFoundException($"Route: {request.RouteId} not found");
        }
        
        route.Distance = request.CalculatedDistance;
        
        await context.SaveChangesAsync(ct);
    }
}