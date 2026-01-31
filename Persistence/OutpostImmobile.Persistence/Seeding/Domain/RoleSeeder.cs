using Microsoft.AspNetCore.Identity;
using OutpostImmobile.Persistence.Common.Consts;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class RoleSeeder
{
    

    public static async ValueTask SeedRolesAsync(
        UserManager<UserInternal> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = Enum.GetNames<UserRoles>();

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole<Guid>(roleName)
                );

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create role {roleName}: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description))
                    );
                }
            }
        }

        var adminUser = await userManager.FindByEmailAsync(AdminConsts.AdminEmail);

        if (adminUser == null)
        {
            adminUser = new UserInternal
            {
                UserName = AdminConsts.AdminEmail,
                Email = AdminConsts.AdminEmail,
                EmailConfirmed = true // important for login
            };

            var userResult = await userManager.CreateAsync(
                adminUser,
                AdminConsts.AdminPassword
            );

            if (!userResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to create admin user: " +
                    string.Join(", ", userResult.Errors.Select(e => e.Description))
                );
            }

            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                nameof(UserRoles.Admin)
            );

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to assign admin role: " +
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }
        }

        await EnsureUserWithRoleAsync(userManager, "courier@outpost.com", "courier", nameof(UserRoles.Courier));
        await EnsureUserWithRoleAsync(userManager, "user@outpost.com", "user", nameof(UserRoles.User));
    }

    private static async Task EnsureUserWithRoleAsync(
        UserManager<UserInternal> userManager,
        string email,
        string password,
        string roleName)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            existingUser = new UserInternal
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(existingUser, password);
            if (!createResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to create user '{email}': " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description))
                );
            }
        }

        if (!await userManager.IsInRoleAsync(existingUser, roleName))
        {
            var roleResult = await userManager.AddToRoleAsync(existingUser, roleName);
            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to assign role '{roleName}' to user '{email}': " +
                    string.Join(", ", roleResult.Errors.Select(e => e.Description))
                );
            }
        }
    }
}