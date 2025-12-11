using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OutpostImmobile.Persistence.Domain.StaticEnums;

public class StaticEnumEntity
{
    [Key]
    public string EnumName { get; set; }
    public ICollection<StaticEnumTranslationEntity> Translations { get; set; }
}

internal class StaticEnumEntityConfiguration : IEntityTypeConfiguration<StaticEnumEntity>
{
    public void Configure(EntityTypeBuilder<StaticEnumEntity> builder)
    {
        builder.HasKey(x => x.EnumName);

        builder
            .HasMany(x => x.Translations)
            .WithOne();
    }
}