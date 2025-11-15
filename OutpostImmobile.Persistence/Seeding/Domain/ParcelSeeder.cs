using Bogus;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class ParcelSeeder
{
    public static async ValueTask SeedParcelsAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.Parcels.Any())
        {
            return;
        }

        var externalUsers = await context.UsersExternal.ToListAsync(ct);
        var maczkopats = await context.Maczkopats.ToListAsync(ct);
        
        var faker = new Faker<ParcelEntity>();

        faker
            .RuleFor(x => x.Product, f => f.Commerce.Product())
            .RuleFor(x => x.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(x => x.UpdatedAt, f => DateTime.UtcNow)
            .RuleFor(x => x.FromUserExternalId, f => f.PickRandom(externalUsers.Select(x => x.Id)))
            .RuleFor(x => x.ReceiverUserExternalId, f => f.PickRandom(externalUsers.Select(x => x.Id)))
            .RuleFor(x => x.Maczkopat, f => f.PickRandom(maczkopats));
        
        var parcels = faker.Generate(10000);
        await context.AddRangeAsync(parcels, ct);
        await context.SaveChangesAsync(ct);
    }
}