namespace OutpostImmobile.Core.Maczkopats.QueryResults;

public record MaczkopatDetailsDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required long Capacity { get; init; }
    public required string AreaName { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Street { get; init; }
    public required string CountryCode { get; init; }
    public required string BuildingNumber { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? UpdatedAt { get; init; }
}
