using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Domain.StaticEnums;

public class StaticEnumTranslations
{
    public required string Key { get; init; }
    
    public required string Translation { get; set; }
    public required TranslationLanguage TranslationLanguage { get; init; }
    
    public StaticEnumEntity EnumEntity { get; set; }
}

internal class StaticEnumEntityTranslationsConfiguration : IEntityTypeConfiguration<StaticEnumTranslations>
{
    public void Configure(EntityTypeBuilder<StaticEnumTranslations> builder)
    {
        builder.HasKey(x => x.Key);
    }
}