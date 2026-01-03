using NetTopologySuite.Geometries;

namespace OutpostImmobile.Persistence.Domain;

public class AddressEntity
{
    public long Id { get; set; }
    public string? Alias { get; set; }

    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Street { get; set; }
    public required string CountryCode { get; set; }
    public required string BuildingNumber { get; set; }
    
    public required Point Location { get; set; }
}