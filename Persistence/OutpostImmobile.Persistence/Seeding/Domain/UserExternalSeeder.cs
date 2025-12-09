using Bogus;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class UserExternalSeeder
{
    public static async ValueTask SeedExternalUserAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.UsersExternal.Any())
        {
            return;
        }
        
        var faker = new Faker<UserExternal>("pl");
        
        faker
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber());

        var users = faker.Generate(100);
        
        context.AddRange(users);
        
        await context.SaveChangesAsync(ct);
    }
}