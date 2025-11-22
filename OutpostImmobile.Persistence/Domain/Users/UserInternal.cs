using Microsoft.AspNetCore.Identity;

namespace OutpostImmobile.Persistence.Domain.Users;

public class UserInternal : IdentityUser<Guid>
{
    public UserRoles Role { get; set; }
    public Guid RoleId { get; set; }
}