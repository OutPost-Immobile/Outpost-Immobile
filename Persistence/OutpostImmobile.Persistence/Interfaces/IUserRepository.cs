using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Tuple<string, string>>> GetUserEmailCredentials(IEnumerable<Guid> userIds);
    Task UpdateUserRoleAsync(string userEmail, UserRoleName roleName);
    Task<UserInternal> LoginUserAsync(string email, string password);
}