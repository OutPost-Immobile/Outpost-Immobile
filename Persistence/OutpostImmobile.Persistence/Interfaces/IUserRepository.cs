using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Tuple<string, string>>> GetUserEmailCredentials(IEnumerable<Guid> userIds);
    Task UpdateUserRoleAsync(string userEmail, UserRoleName roleName);
}