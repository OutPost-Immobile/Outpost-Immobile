using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence;

namespace OutpostImmobile.Core.Tests;

public class MockDbContextFactory : IDbContextFactory<OutpostImmobileDbContext>
{
    private readonly DbContextOptions<OutpostImmobileDbContext> _options;

    public MockDbContextFactory(string dbName)
    {
        _options = new DbContextOptionsBuilder<OutpostImmobileDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;
    }
    
    public OutpostImmobileDbContext CreateDbContext()
    {
        var context = new OutpostImmobileDbContext(_options);
        context.IsInTestEnv = true;
        return context;
    }
    
    public Task<OutpostImmobileDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        var context = new OutpostImmobileDbContext(_options);
        context.IsInTestEnv = true;
        return Task.FromResult(context);
    }
}