using Bogus;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class RoutesSeeder
{
    public static async ValueTask SeedRoutesAsync(OutpostImmobileDbContext context, CancellationToken ct = default)
    {
        if (context.Routes.Any())
        {
            return;
        }

        var addressIds = await context.Addresses
            .Select(x => new
            {
                x.Id,
                Name = $"{x.City} {x.Street} {x.BuildingNumber}"
            })
            .ToListAsync(ct);
        
        var faker = new Faker<RouteEntity>("pl");

        faker
            .RuleFor(x => x.StartAddressId, f => f.PickRandom(addressIds).Id)
            .RuleFor(x => x.EndAddressId, f => f.PickRandom(addressIds).Id)
            .RuleFor(x => x.StartAddressName, f => f.PickRandom(addressIds).Name)
            .RuleFor(x => x.EndAddressName, f => f.PickRandom(addressIds).Name)
            .RuleFor(x => x.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, f => DateTime.UtcNow);
        
        var routes = faker.Generate(10000);
        await context.AddRangeAsync(routes, ct);
        await context.SaveChangesAsync(ct);
    }
}