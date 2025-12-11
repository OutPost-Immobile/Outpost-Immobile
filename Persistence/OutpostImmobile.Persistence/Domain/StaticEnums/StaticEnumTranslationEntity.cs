using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Domain.StaticEnums;

public class StaticEnumTranslationEntity
{
    public required int EnumValue { get; set; }
    public required string EnumName { get; set; }
    public required string Translation { get; set; }
    public required TranslationLanguage TranslationLanguage { get; init; }
    
    public StaticEnumEntity EnumEntity { get; set; }
}

internal class StaticEnumEntityTranslationsConfiguration : IEntityTypeConfiguration<StaticEnumTranslationEntity>
{
    public void Configure(EntityTypeBuilder<StaticEnumTranslationEntity> builder)
    {
        builder.HasKey(x => new { x.EnumValue, x.EnumName, x.TranslationLanguage });

        builder.Property(x => x.TranslationLanguage)
            .HasConversion<string>();
    }
}