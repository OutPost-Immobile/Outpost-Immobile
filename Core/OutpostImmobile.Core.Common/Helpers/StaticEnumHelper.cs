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
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContext;

    public StaticEnumHelper(IDbContextFactory<OutpostImmobileDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<int, string>> GetStaticEnumTranslations(string enumName, TranslationLanguage language)
    {
        await using var context = await _dbContext.CreateDbContextAsync();
        
        return await context.StaticEnums
            .Where(x => x.EnumName == enumName)
            .SelectMany(x => x.Translations)
            .Select(x => new { x.EnumValue, x.Translation })
            .ToDictionaryAsync(x => x.EnumValue, x => x.Translation);
    }
}