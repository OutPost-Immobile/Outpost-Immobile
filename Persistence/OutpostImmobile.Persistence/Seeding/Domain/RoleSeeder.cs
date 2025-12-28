using Microsoft.AspNetCore.Identity;
using OutpostImmobile.Persistence.Common.Consts;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class RoleSeeder
{
    

    public static async ValueTask SeedRoles(UserManager<UserInternal> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new List<UserRoles>
        {
            UserRoles.Admin,
            UserRoles.Manager,
            UserRoles.Courier,
            UserRoles.User
        };
        
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
        var adminUser = await userManager.FindByEmailAsync(AdminConsts.AdminEmail);
        if (adminUser == null)
        {
            adminUser = new UserInternal
            {
                UserName = AdminConsts.AdminEmail,
                Email = AdminConsts.AdminEmail,
            };

            await userManager.CreateAsync(adminUser, AdminConsts.AdminPassword);
            await userManager.AddToRoleAsync(adminUser, nameof(UserRoles.Admin));
        }
    }
}