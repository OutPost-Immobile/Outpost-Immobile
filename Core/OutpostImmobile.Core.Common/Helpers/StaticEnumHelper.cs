using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Core.Common.Helpers;

public interface IStaticEnumHelper
{
    Task<Dictionary<int, string>> GetStaticEnumTranslations<TEnum>(TEnum enumType, TranslationLanguage language) where TEnum : Enum;
}

public class StaticEnumHelper : IStaticEnumHelper
{
    private readonly OutpostImmobileDbContext _dbContext;

    public StaticEnumHelper(OutpostImmobileDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<int, string>> GetStaticEnumTranslations<TEnum>(TEnum enumType, TranslationLanguage language) where TEnum : Enum
    {
        return await _dbContext.StaticEnumTranslations
            .Where(x => x.EnumName == nameof(enumType) && x.TranslationLanguage == language)
            .Select(x => new { x.EnumValue, x.Translation })
            .ToDictionaryAsync(x => x.EnumValue, x => x.Translation);
    }
}