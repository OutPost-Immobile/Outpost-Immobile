using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Seeding.StaticEnums;
using OutpostImmobile.Persistence.Seeding.StaticEnums.EnumTranslations;

namespace OutpostImmobile.Persistence.Seeding;

public class ApplicationSeeder
{
    public static async Task SeedAsync(OutpostImmobileDbContext context, CancellationToken ct = default)
    {
        await StaticEnumSeeder.SeedAsync<ParcelStatus>(context, StaticEnumTranslations.GetParcelStatusTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<PayloadSize>(context, StaticEnumTranslations.GetPayloadSizeTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<MaczkopatEventLogType>(context, StaticEnumTranslations.GetMaczkopatEventLogTypeTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<ParcelEventLogType>(context, StaticEnumTranslations.GetParcelEventLogTypeTranslations(), ct);
    }
}