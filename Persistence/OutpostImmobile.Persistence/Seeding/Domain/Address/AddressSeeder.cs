using Bogus;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain.Address;

public class AddressSeeder
{
    public static async ValueTask SeedAddressesAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.Addresses.Any())
        {
            return;
        }
        
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        var csvContent = AddressImporter.Import();

        var addresses = csvContent.Select(x => new AddressEntity
        {
            City = x.City,
            PostalCode = x.PostalCode,
            Street = x.Street,
            CountryCode = "Pl",
            BuildingNumber = x.HouseNumber,
            Location = geometryFactory.CreatePoint(new Coordinate(x.Longitude, x.Latitude))
        }).ToList();

        await context.AddRangeAsync(addresses, ct);
        await context.SaveChangesAsync(ct);
    }
}