using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Maczkopats.Commands;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Fixture for maczkopat-related operations using Commands and Queries.
/// Used for testing maczkopat state updates and log management.
/// </summary>
public class MaczkopatFixture : BaseFixture
{
    // Private fields for FitSharp Script table setters
    private string? _maczkopatId;
    private string? _logType;
    
    #region FitSharp Setter Methods (for Script tables: | set X | value |)
    
    public void SetMaczkopatId(string value) => _maczkopatId = value;
    public void SetLogType(string value) => _logType = value;
    
    #endregion
    
    /// <summary>
    /// Add a log entry to maczkopat using MaczkopatAddLogCommand
    /// </summary>
    public bool AddMaczkopatLog()
    {
        var maczkopatId = _maczkopatId != null 
            ? Guid.Parse(_maczkopatId) 
            : Context.CurrentMaczkopatId ?? Guid.Empty;
        
        if (maczkopatId == Guid.Empty)
        {
            return false;
        }
        
        try
        {
            var logType = ParseLogType(_logType ?? "LockerOpened");
            
            var command = new MaczkopatAddLogCommand
            {
                MaczkopatId = maczkopatId,
                LogType = logType
            };
            
            var result = Context.Mediator.Send<MaczkopatAddLogCommand, Task>(command);
            result.GetAwaiter().GetResult();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Get maczkopat capacity
    /// </summary>
    public long GetMaczkopatCapacity(string maczkopatId)
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        if (!Guid.TryParse(maczkopatId, out var id))
        {
            return 0;
        }
        
        var maczkopat = context.Maczkopats
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == id);
        
        return maczkopat?.Capacity ?? 0;
    }
    
    /// <summary>
    /// Get count of parcels currently in maczkopat
    /// </summary>
    public int GetParcelsInMaczkopatCount(string maczkopatId)
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        if (!Guid.TryParse(maczkopatId, out var id))
        {
            return 0;
        }
        
        return context.Parcels
            .Where(p => p.MaczkopatEntityId == id && p.Status == ParcelStatus.InMaczkopat)
            .Count();
    }
    
    /// <summary>
    /// Check if maczkopat has space for more parcels
    /// </summary>
    public bool MaczkopatHasAvailableSpace(string maczkopatId)
    {
        var capacity = GetMaczkopatCapacity(maczkopatId);
        var parcelsCount = GetParcelsInMaczkopatCount(maczkopatId);
        
        return parcelsCount < capacity;
    }
    
    /// <summary>
    /// Get maczkopat code by ID
    /// </summary>
    public string GetMaczkopatCode(string maczkopatId)
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        if (!Guid.TryParse(maczkopatId, out var id))
        {
            return "NotFound";
        }
        
        var maczkopat = context.Maczkopats
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == id);
        
        return maczkopat?.Code ?? "NotFound";
    }
    
    /// <summary>
    /// Get maczkopat log entries count
    /// </summary>
    public int GetMaczkopatLogsCount(string maczkopatId)
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        if (!Guid.TryParse(maczkopatId, out var id))
        {
            return 0;
        }
        
        return context.MaczkopatEventLogs
            .Where(l => l.MaczkopatId == id)
            .Count();
    }
    
    /// <summary>
    /// Set the current maczkopat context
    /// </summary>
    public void SelectMaczkopat(string maczkopatId)
    {
        if (Guid.TryParse(maczkopatId, out var id))
        {
            Context.CurrentMaczkopatId = id;
        }
    }
    
    private MaczkopatEventLogType ParseLogType(string logType)
    {
        return logType switch
        {
            "LockerOpened" => MaczkopatEventLogType.LockerOpened,
            "LockerClosed" => MaczkopatEventLogType.LockerClosed,
            "OpenedByForce" => MaczkopatEventLogType.OpenedByForce,
            "Error" => MaczkopatEventLogType.Error,
            _ => MaczkopatEventLogType.LockerOpened
        };
    }
}
