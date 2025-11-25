namespace OutpostImmobile.Core.Parcels.QueryResults;

public record ParcelDto
{
    public required List<Guid> ParcelIds { get; init; }
}