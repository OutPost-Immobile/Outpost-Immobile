using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Core.Common.Helpers;

public interface IStaticEnumHelper
{
    Task<Dictionary<int, string>> GetStaticEnumTranslations(string enumName, TranslationLanguage language);
}

public class StaticEnumHelper : IStaticEnumHelper
{
    private readonly OutpostImmobileDbContext _dbContext;

    public StaticEnumHelper(OutpostImmobileDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<int, string>> GetStaticEnumTranslations(string enumName, TranslationLanguage language)
    {
        return await _dbContext.StaticEnums
            .Where(x => x.EnumName == enumName)
            .SelectMany(x => x.Translations)
            .Select(x => new { x.EnumValue, x.Translation })
            .ToDictionaryAsync(x => x.EnumValue, x => x.Translation);
    }
}