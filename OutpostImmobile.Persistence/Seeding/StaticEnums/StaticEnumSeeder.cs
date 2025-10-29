using OutpostImmobile.Persistence.Domain.StaticEnums;

namespace OutpostImmobile.Persistence.Seeding.StaticEnums;

public class StaticEnumSeeder
{
    public static async Task SeedAsync<TEnum>(OutpostImmobileDbContext context, TEnum staticEnum, 
        List<StaticEnumTranslationEntity> translations, CancellationToken ct) where TEnum : Enum
    {
        
    }
}