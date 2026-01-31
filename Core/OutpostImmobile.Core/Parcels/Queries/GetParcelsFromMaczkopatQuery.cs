using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Core.Parcels.Queries;

public record GetParcelsFromMaczkopatQuery : IRequest<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    public Guid MaczkopatId { get; init; }
}

internal class GetParcelsFromMaczkopatQueryHandler : IRequestHandler<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IStaticEnumHelper _staticEnumHelper;
    public GetParcelsFromMaczkopatQueryHandler(IStaticEnumHelper staticEnumHelper, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _staticEnumHelper = staticEnumHelper;
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<List<ParcelDto>> Handle(GetParcelsFromMaczkopatQuery request, CancellationToken ct)
    {
        var staticTranslations = await _staticEnumHelper.GetStaticEnumTranslations(nameof(ParcelStatus), TranslationLanguage.Pl);
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        return await context.Parcels
            .Where(p => p.MaczkopatEntityId == request.MaczkopatId)
            .Select(x => new ParcelDto
            {
                FriendlyId = x.FriendlyId,
                Status = x.Status != null ? staticTranslations[(int)x.Status] :  null,
            })
            .ToListAsync(ct);
    }
}