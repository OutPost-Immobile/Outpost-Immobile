using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Maczkopats.QueryResults;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Maczkopats.Queries;

public record GetMaczkopatByIdQuery : IRequest<GetMaczkopatByIdQuery, Task<MaczkopatDetailsDto>>
{
    public required Guid MaczkopatId { get; init; }
}

internal class GetMaczkopatByIdQueryHandler : IRequestHandler<GetMaczkopatByIdQuery, Task<MaczkopatDetailsDto>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public GetMaczkopatByIdQueryHandler(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<MaczkopatDetailsDto> Handle(GetMaczkopatByIdQuery request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var maczkopat = await context.Maczkopats
            .Where(m => m.Id == request.MaczkopatId)
            .Select(m => new MaczkopatDetailsDto
            {
                Id = m.Id,
                Code = m.Code,
                Capacity = m.Capacity,
                AreaName = m.Area.AreaName,
                City = m.Address.City,
                PostalCode = m.Address.PostalCode,
                Street = m.Address.Street,
                CountryCode = m.Address.CountryCode,
                BuildingNumber = m.Address.BuildingNumber,
                CreatedAt = m.CreatedAt.Value,
                UpdatedAt = m.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);

        return maczkopat ?? throw new EntityNotFoundException($"Maczkopat with id {request.MaczkopatId} not found.");
    }
}
