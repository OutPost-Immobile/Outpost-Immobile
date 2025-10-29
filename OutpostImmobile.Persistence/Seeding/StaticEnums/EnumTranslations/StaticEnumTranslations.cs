using OutpostImmobile.Persistence.Domain.StaticEnums;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Seeding.StaticEnums.EnumTranslations;

public class StaticEnumTranslations
{
    public static List<StaticEnumTranslationEntity> GetParcelStatusTranslations()
    {
        return new List<StaticEnumTranslationEntity>
        {
            new()
            {
                EnumValue = (int)ParcelStatus.Experighted,
                Translation = "Przeterminowana",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.Experighted),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.Delivered,
                Translation = "Dostarczona",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.Delivered),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.InTransit,
                Translation = "W transporcie",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.InTransit),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.InMagazine,
                Translation = "W magazynie",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.InMagazine),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.Forgotten,
                Translation = "Zapomniana",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.Forgotten),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.Deleted,
                Translation = "Usunięta",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.Deleted),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.Sent,
                Translation = "Wysłana",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.Sent),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.ToReturn,
                Translation = "Do zwrotu",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.ToReturn),
            },
            new()
            {
                EnumValue = (int)ParcelStatus.SendToStorage,
                Translation = "Wysłana do magazynu",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(ParcelStatus.SendToStorage),
            },
        };
    }
    
    public static List<StaticEnumTranslationEntity> GetPayloadSizeTranslations()
    {
        return new List<StaticEnumTranslationEntity>
        {
            new()
            {
                EnumValue = (int)PayloadSize.Small,
                Translation = "Mały",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(PayloadSize.Small),
            },
            new()
            {
                EnumValue = (int)PayloadSize.Medium,
                Translation = "Średni",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(PayloadSize.Medium),
            },
            new()
            {
                EnumValue = (int)PayloadSize.Large,
                Translation = "Duży",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(PayloadSize.Large),
            },
            new()
            {
                EnumValue = (int)PayloadSize.XLarge,
                Translation = "Bardzo duży",
                TranslationLanguage = TranslationLanguage.Pl,
                EnumName = nameof(PayloadSize.XLarge),
            },
        };
    }
}