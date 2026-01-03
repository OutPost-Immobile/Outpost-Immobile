using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;

    public UserRepository(IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
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
}