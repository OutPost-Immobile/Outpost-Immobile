namespace OutpostImmobile.Core.Parcels.QueryResults;

public record ParcelLogDto
{
    public required string? Message { get; init; }
    public required string? ParcelEventLogType { get; init; }
    public DateTime? CreatedAt { get; init; }
}