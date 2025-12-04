using OutpostImmobile.Core.Paralizator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Core.Parcels.Queries;

public record GetParcelsFromMaczkopatQuery : IRequest<GetParcelsFromMaczkopatQuery, List<ParcelDto>>
{
    public Guid MaczkopatId { get; init; }
}

internal class GetParcelsFromMaczkopatQueryHandler : IRequestHandler<GetParcelsFromMaczkopatQuery, List<ParcelDto>>
{
    public Task<List<ParcelDto>> Handle(GetParcelsFromMaczkopatQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}