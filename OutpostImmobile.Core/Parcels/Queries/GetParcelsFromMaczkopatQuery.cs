using DispatchR.Abstractions.Send;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Core.Parcels.Queries;

public record GetParcelsFromMaczkopatQuery : IRequest<ParcelDto, Task>
{
    public Guid MaczkopatId { get; init; }
}

internal class GetParcelsFromMaczkopatQueryHandler : IRequestHandler<GetParcelsFromMaczkopatQuery, Task<ParcelDto>>
{
    public Task<ParcelDto> Handle(GetParcelsFromMaczkopatQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}