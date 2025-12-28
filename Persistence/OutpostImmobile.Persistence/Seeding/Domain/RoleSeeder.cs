using Microsoft.AspNetCore.Identity;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class RoleSeeder
{
    private static readonly UserRoles[] roles =
    {
        UserRoles.Admin,
        UserRoles.Manager,
        UserRoles.Courier,
        UserRoles.User
    };

    public static async ValueTask SeedRoles(UserManager<UserInternal> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in roles) 
        {
            string roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
            }
        }
        string adminEmail = "admin@outpost.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new UserInternal
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            await userManager.CreateAsync(adminUser, "admin");
            await userManager.AddToRoleAsync(adminUser, nameof(UserRoles.Admin));
        }
    }
}