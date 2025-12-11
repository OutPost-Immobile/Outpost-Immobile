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

    public Task<Dictionary<int, string>> GetStaticEnumTranslations<TEnum>(TEnum enumType, TranslationLanguage language) where TEnum : Enum
    {
        throw new NotImplementedException();
    }
}