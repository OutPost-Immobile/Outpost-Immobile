using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Parcels.QueryResults;

public record ParcelDto
{
    public required Guid ParcelId { get; init; }
    public ParcelStatus Status { get; set; }
}