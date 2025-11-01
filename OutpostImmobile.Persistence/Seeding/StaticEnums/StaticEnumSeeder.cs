using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain.StaticEnums;

namespace OutpostImmobile.Persistence.Seeding.StaticEnums;

public class StaticEnumSeeder
{
    public static async Task SeedAsync<TEnum>(OutpostImmobileDbContext context,
    List<StaticEnumTranslationEntity> translations, CancellationToken ct) where TEnum : Enum
    {
        var enumName = typeof(TEnum).Name;
        
        var existingEnum = await context.StaticEnums
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.EnumName == enumName, ct);

        if (existingEnum == null)
        {
            var enumToAdd = new StaticEnumEntity
            {
                EnumName = enumName,
                Translations = translations
            };
            context.Add(enumToAdd);
        }
        else
        {
            foreach (var newTranslation in translations)
            {
                var existing = existingEnum.Translations
                    .FirstOrDefault(x => x.EnumValue == newTranslation.EnumValue
                                         && x.TranslationLanguage == newTranslation.TranslationLanguage);

                if (existing != null)
                {
                    existing.Translation = newTranslation.Translation;
                }
                else
                {
                    existingEnum.Translations.Add(newTranslation);
                }
            }
        }

        await context.SaveChangesAsync(ct);
    }
}