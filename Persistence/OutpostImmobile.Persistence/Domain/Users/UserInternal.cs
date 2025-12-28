using Microsoft.AspNetCore.Identity;

namespace OutpostImmobile.Persistence.Domain.Users;

public class UserInternal : IdentityUser<Guid>
{
    public long RouteId { get; set; }
    public RouteEntity Route { get; set; }
    
}