using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Parcels.QueryResults;

public record ParcelDto
{
    public required string FriendlyId { get; init; }
    public required string? Status { get; init; }
}