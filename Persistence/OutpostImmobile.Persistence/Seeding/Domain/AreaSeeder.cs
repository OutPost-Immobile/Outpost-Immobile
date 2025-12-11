using Bogus;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class AreaSeeder
{
    public static async ValueTask SeedAreasAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.Areas.Any())
        {
            return;
        }

        var faker = new Faker<AreaEntity>("pl");

        faker.RuleFor(x => x.AreaName, x => x.Company.CompanyName());
        
        var areas = faker.Generate(1000);
        
        await context.AddRangeAsync(areas, ct);
        await context.SaveChangesAsync(ct);
    }
}