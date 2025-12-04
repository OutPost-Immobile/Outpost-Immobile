using Bogus;
using OutpostImmobile.Persistence.Domain;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class NumberTemplateSeeder
{
    public static async ValueTask SeedNumberTemplatesAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.NumberTemplates.Any())
        {
            return;
        }

        var faker = new Faker<NumberTemplateEntity>("pl");

        faker.RuleFor(x => x.TemplateNumber, GetRandomPlateNumber(8));

        var plates = faker.Generate(1000);
        
        await context.NumberTemplates.AddRangeAsync(plates, ct);
        await context.SaveChangesAsync(ct);
    }
    
    private static string GetRandomPlateNumber(int length)
    {
        var random = new Random();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}