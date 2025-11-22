using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Interceptors;

public class AuditableEntityInterceptor :  SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken)
    {
        var dbContext = (OutpostImmobileDbContext?) eventData.Context;
    
        if (dbContext is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);    
        }
        
        var entries = dbContext.ChangeTracker.Entries<AuditableEntity>().ToList();
    
        if (!entries.Any())
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    
        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var userId = await GetAuthorizedUserIdentifier(dbContext);
    
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedById = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);    
    }
    
    private async Task<Guid?> GetAuthorizedUserIdentifier(OutpostImmobileDbContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
    
        if (httpContext is null)
        {
            return null;
        }
    
        var authenticateResult = await httpContext.AuthenticateAsync();
    
        var name = authenticateResult.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return name is not null
            ? (await context.UsersInternal.FirstOrDefaultAsync(x => x.UserName == name))?.Id
            : null;
    }
}