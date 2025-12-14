using OutpostImmobile.Core.Common.Helpers;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Parcels.Queries;

public record GetParcelsFromMaczkopatQuery : IRequest<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    public Guid MaczkopatId { get; init; }
}

internal class GetParcelsFromMaczkopatQueryHandler : IRequestHandler<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    private readonly IParcelRepository _parcelRepository;
    //private readonly IStaticEnumHelper _staticEnumHelper;
    public GetParcelsFromMaczkopatQueryHandler(IParcelRepository parcelRepository)
    {
        _parcelRepository = parcelRepository;
        //_staticEnumHelper = staticEnumHelper;
    }
    
    public async Task<List<ParcelDto>> Handle(GetParcelsFromMaczkopatQuery request, CancellationToken cancellationToken)
    {
        var parcels = await _parcelRepository.GetParcelsFromMaczkopatAsync(request.MaczkopatId);
        //var staticTranslations = await _staticEnumHelper.GetStaticEnumTranslations(ParcelStatus, TranslationLanguage.Pl);
        return parcels.Select(x => new ParcelDto
        {
            FriendlyId = x.FriendlyId,
            Status = string.Empty,
        }).ToList();
    }
}