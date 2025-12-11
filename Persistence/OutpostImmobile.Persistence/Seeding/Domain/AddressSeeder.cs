using Bogus;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class AddressSeeder
{
    public static async ValueTask SeedAddressesAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.Addresses.Any())
        {
            return;
        }

        var faker = new Faker<AddressEntity>("pl");

        faker
            .RuleFor(x => x.BuildingNumber, b => b.PickRandom(1000))
            .RuleFor(x => x.City, c => c.Address.City())
            .RuleFor(x => x.Street, x => x.Address.StreetName())
            .RuleFor(x => x.PostalCode, x => x.Address.ZipCode())
            .RuleFor(x => x.CountryCode, x => x.Address.CountryCode())
            .RuleFor(x => x.Alias, x => x.Address.FullAddress())
            .RuleFor(x => x.Location, x => new Point(x.Address.Latitude(), x.Address.Longitude())
            {
                SRID = 4326
            });
        
        var addresses = faker.Generate(1000);

        await context.AddRangeAsync(addresses, ct);
        await context.SaveChangesAsync(ct);
    }
}