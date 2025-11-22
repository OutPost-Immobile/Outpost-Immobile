using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Common.Helpers;

public class EnumKeyGenerator
{
    public static string GenerateKey<TEnum>(TEnum staticEnum, TranslationLanguage language) where TEnum : Enum
    {
        return $"{typeof(TEnum).Name}_{nameof(staticEnum)}_Translation_{nameof(language)}";
    }
}