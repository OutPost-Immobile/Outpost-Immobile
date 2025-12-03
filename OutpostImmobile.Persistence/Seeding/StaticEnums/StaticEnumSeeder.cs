using OutpostImmobile.Persistence.Domain.StaticEnums;

namespace OutpostImmobile.Persistence.Seeding.StaticEnums;

public class StaticEnumSeeder
{
    public static async Task SeedAsync<TEnum>(OutpostImmobileDbContext context,
        List<StaticEnumTranslationEntity> translations, CancellationToken ct) where TEnum : Enum
    {
        throw new NotImplementedException();
    }
}