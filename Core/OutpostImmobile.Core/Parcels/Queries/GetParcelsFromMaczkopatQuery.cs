using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Core.Parcels.Queries;

public record GetParcelsFromMaczkopatQuery : IRequest<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    public Guid MaczkopatId { get; init; }
}

internal class GetParcelsFromMaczkopatQueryHandler : IRequestHandler<GetParcelsFromMaczkopatQuery, Task<List<ParcelDto>>>
{
    public async Task<List<ParcelDto>> Handle(GetParcelsFromMaczkopatQuery request, CancellationToken cancellationToken)
    {
        return [];
    }
}