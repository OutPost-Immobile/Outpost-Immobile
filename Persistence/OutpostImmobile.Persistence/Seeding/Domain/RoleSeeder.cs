using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Seeding.Domain;

public class RoleSeeder
{
    public static async ValueTask SeedRolesAsync(OutpostImmobileDbContext context, CancellationToken ct)
    {
        if (context.UserRoles.Any())
        {
            return;
        }
        
        var rolesList = new List<UserRoles>
        {
            new()
            {
                RoleName = UserRoleNames.Admin
            },
            new()
            {
                RoleName = UserRoleNames.Client
            },
            new()
            {
                RoleName = UserRoleNames.Courier
            },
            new()
            {
                RoleName = UserRoleNames.Manager
            }
        };
        
        context.AddRange(rolesList);
        await context.SaveChangesAsync(ct);
    }
}