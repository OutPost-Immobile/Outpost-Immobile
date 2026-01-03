using Microsoft.AspNetCore.Identity;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Seeding.Domain;
using OutpostImmobile.Persistence.Seeding.Domain.Address;
using OutpostImmobile.Persistence.Seeding.StaticEnums;
using OutpostImmobile.Persistence.Seeding.StaticEnums.EnumTranslations;

namespace OutpostImmobile.Persistence.Seeding;

public class ApplicationSeeder
{
    public static async Task SeedAsync(
        OutpostImmobileDbContext context, 
        UserManager<UserInternal> userManager, 
        RoleManager<IdentityRole<Guid>> roleManager, 
        CancellationToken ct = default)
    {
        await StaticEnumSeeder.SeedAsync<ParcelStatus>(context, StaticEnumTranslations.GetParcelStatusTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<PayloadSize>(context, StaticEnumTranslations.GetPayloadSizeTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<MaczkopatEventLogType>(context, StaticEnumTranslations.GetMaczkopatEventLogTypeTranslations(), ct);
        await StaticEnumSeeder.SeedAsync<ParcelEventLogType>(context, StaticEnumTranslations.GetParcelEventLogTypeTranslations(), ct);
        await RoleSeeder.SeedRolesAsync(userManager, roleManager);
        await UserExternalSeeder.SeedExternalUserAsync(context, ct);
        await AreaSeeder.SeedAreasAsync(context, ct);
        await AddressSeeder.SeedAddressesAsync(context, ct);
        await MaczkopatSeeder.SeedMaczkopatsAsync(context, ct);
        await ParcelSeeder.SeedParcelsAsync(context, ct);
        await NumberTemplateSeeder.SeedNumberTemplatesAsync(context, ct);
    }
}