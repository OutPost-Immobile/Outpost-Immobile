using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Api.Request;

public class UpdateRoleRequest
{
    public required string Email { get; init; }
    public required UserRoleName RoleName { get; init; }
}