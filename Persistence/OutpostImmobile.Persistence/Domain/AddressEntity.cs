using NetTopologySuite.Geometries;

namespace OutpostImmobile.Persistence.Domain;

public class AddressEntity
{
    public long Id { get; set; }
    public string Alias { get; set; }

    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public string CountryCode { get; set; }
    public int BuildingNumber { get; set; }
    
    public Point Location { get; set; }
}