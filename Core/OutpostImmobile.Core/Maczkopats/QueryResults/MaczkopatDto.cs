namespace OutpostImmobile.Core.Maczkopats.QueryResults;

public record MaczkopatDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required long Capacity { get; init; }
    public required string AreaName { get; init; }
    public required string City { get; init; }
    public required string Street { get; init; }
    public required string BuildingNumber { get; init; }
    public required int ParcelCount { get; init; }
}
