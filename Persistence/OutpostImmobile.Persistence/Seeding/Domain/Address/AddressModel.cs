using CsvHelper.Configuration.Attributes;

namespace OutpostImmobile.Persistence.Seeding.Domain.Address;

public record AddressModel
{
    [Name("lat")]
    public required double Latitude { get; init; }
    
    [Name("lon")]
    public required double Longitude { get; init; }
    
    [Name("city")]
    public required string City { get; init; }
    
    [Name("street")]
    public required string Street { get; init; }
    
    [Name("housenumber")]
    public required string HouseNumber { get; init; }
    
    [Name("postcode")]
    public required string PostalCode { get; init; }
}