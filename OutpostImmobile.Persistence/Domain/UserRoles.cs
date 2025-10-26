using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Domain;

public class UserRoles
{
    public Guid Id { get; }
    public UserRoleNames RoleName { get; set; }
}