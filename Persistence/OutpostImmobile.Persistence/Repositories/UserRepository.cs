using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly UserManager<UserInternal> _userManager; 

    public UserRepository(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory, UserManager<UserInternal> userManager)
    {
        _userManager = _userManager;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Tuple<string, string>>> GetUserEmailCredentials(IEnumerable<Guid> userIds)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var usersInternal = await context.UsersInternal
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new Tuple<string, string>(x.Email, x.UserName))
            .ToListAsync();
        
        var usersExternal = await context.UsersExternal
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new Tuple<string, string>(x.Email, x.Name))
            .ToListAsync();

        return usersInternal.Concat(usersExternal);
    }

    public async Task UpdateUserRoleAsync(string userEmail, UserRoleName roleName)
    {
        var userToUpdate = await _userManager.FindByEmailAsync(userEmail);

        if (userToUpdate == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        var currentRoles = await _userManager.GetRolesAsync(userToUpdate);
        
        await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
        
        var result = await _userManager.AddToRoleAsync(userToUpdate, roleName.ToString());

        if (!result.Succeeded)
        {
            var error = result.Errors.Select(x => x.Description);
            // for debugging
            throw new Exception($"Failed to update user role: {error}");
        }
    }
}