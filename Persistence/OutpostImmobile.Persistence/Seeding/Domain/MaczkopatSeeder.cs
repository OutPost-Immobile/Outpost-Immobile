using Bogus;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class MaczkopatSeeder
{
    public static async ValueTask SeedMaczkopatsAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if(context.Maczkopats.Any())
        {
            return;
        }

        var faker = new Faker<MaczkopatEntity>("pl");

        var addresses = await context.Addresses.ToListAsync(ct);
        var areas = await context.Areas.ToListAsync(ct);

        faker
            .RuleFor(x => x.Code, x => x.Hacker.Verb())
            .RuleFor(x => x.Address, x => x.PickRandom(addresses))
            .RuleFor(x => x.Area, x => x.PickRandom(areas))
            .RuleFor(x => x.Capacity, x => x.PickRandom(10, 100));

        var maczkopats = faker.Generate(1000);
        
        await context.AddRangeAsync(maczkopats, ct);
        await context.SaveChangesAsync(ct);
    }
}