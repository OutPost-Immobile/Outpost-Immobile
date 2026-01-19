using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;

namespace OutpostImmobile.Core.Tests;

public class MockDbContextFactory : IDbContextFactory<OutpostImmobileDbContext>
{
    private readonly OutpostImmobileDbContext _dbContext;

    public MockDbContextFactory(string dbName)
    {
        var options = new DbContextOptionsBuilder<OutpostImmobileDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging(false)
            .Options;
        
        _dbContext = new OutpostImmobileDbContext(options);
        _dbContext.IsInTestEnv = true;
    }
    
    public OutpostImmobileDbContext CreateDbContext()
    {
        return _dbContext;
    }
}