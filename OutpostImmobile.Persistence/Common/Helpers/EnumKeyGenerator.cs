using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Common.Helpers;

public class EnumKeyGenerator
{
    public static string Generate<TEnum>(TEnum @enum, TranslationLanguage language) where TEnum : Enum
    {
        return $"{typeof(TEnum)}_{nameof(@enum)}_Translation_{nameof(language)}";
    }
}