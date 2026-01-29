using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Parcels.Commands;
using OutpostImmobile.Core.Parcels.Queries;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Fixture for parcel-related operations using Commands and Queries.
/// Used for testing parcel status updates, delivery, and pickup scenarios.
/// </summary>
public class ParcelFixture : BaseFixture
{
    // Private fields for FitSharp Script table setters
    private string? _parcelFriendlyId;
    private string? _newStatus;
    private string? _maczkopatId;
    
    #region FitSharp Setter Methods (for Script tables: | set X | value |)
    
    public void SetParcelFriendlyId(string value) => _parcelFriendlyId = value;
    public void SetNewStatus(string value) => _newStatus = value;
    public void SetMaczkopatId(string value) => _maczkopatId = value;
    
    #endregion
    
    /// <summary>
    /// Update parcel status using BulkUpdateParcelStatusCommand
    /// </summary>
    public bool UpdateParcelStatus()
    {
        if (string.IsNullOrEmpty(_parcelFriendlyId) || string.IsNullOrEmpty(_newStatus))
        {
            return false;
        }
        
        try
        {
            var status = ParseStatus(_newStatus);
            
            var command = new BulkUpdateParcelStatusCommand
            {
                ParcelModels = new[]
                {
                    new BulkUpdateParcelStatusCommand.ParcelModel
                    {
                        FriendlyId = _parcelFriendlyId,
                        Status = status
                    }
                }
            };
            
            var result = Context.Mediator.Send<BulkUpdateParcelStatusCommand, Task>(command);
            result.GetAwaiter().GetResult();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Update multiple parcels to transit status (PU07 - pickup from warehouse)
    /// </summary>
    public bool UpdateParcelsToTransit(string parcelIds)
    {
        try
        {
            var ids = parcelIds.Split(',').Select(x => x.Trim()).ToList();
            var parcelModels = ids.Select(id => new BulkUpdateParcelStatusCommand.ParcelModel
            {
                FriendlyId = id,
                Status = ParcelStatus.InTransit
            }).ToList();
            
            var command = new BulkUpdateParcelStatusCommand
            {
                ParcelModels = parcelModels
            };
            
            var result = Context.Mediator.Send<BulkUpdateParcelStatusCommand, Task>(command);
            result.GetAwaiter().GetResult();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Update multiple parcels to InMaczkopat status (PU07 - delivery to maczkopat)
    /// </summary>
    public bool UpdateParcelsToInMaczkopat(string parcelIds)
    {
        try
        {
            var ids = parcelIds.Split(',').Select(x => x.Trim()).ToList();
            var parcelModels = ids.Select(id => new BulkUpdateParcelStatusCommand.ParcelModel
            {
                FriendlyId = id,
                Status = ParcelStatus.InMaczkopat
            }).ToList();
            
            var command = new BulkUpdateParcelStatusCommand
            {
                ParcelModels = parcelModels
            };
            
            var result = Context.Mediator.Send<BulkUpdateParcelStatusCommand, Task>(command);
            result.GetAwaiter().GetResult();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Update multiple parcels to SendToStorage status (PU08 - send to warehouse)
    /// </summary>
    public bool UpdateParcelsToSendToStorage(string parcelIds)
    {
        try
        {
            var ids = parcelIds.Split(',').Select(x => x.Trim()).ToList();
            var parcelModels = ids.Select(id => new BulkUpdateParcelStatusCommand.ParcelModel
            {
                FriendlyId = id,
                Status = ParcelStatus.SendToStorage
            }).ToList();
            
            var command = new BulkUpdateParcelStatusCommand
            {
                ParcelModels = parcelModels
            };
            
            var result = Context.Mediator.Send<BulkUpdateParcelStatusCommand, Task>(command);
            result.GetAwaiter().GetResult();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Get parcels from maczkopat using GetParcelsFromMaczkopatQuery
    /// </summary>
    public int GetParcelsFromMaczkopatCount()
    {
        var maczkopatId = Context.CurrentMaczkopatId;
        if (maczkopatId == null)
        {
            return 0;
        }
        
        try
        {
            var query = new GetParcelsFromMaczkopatQuery
            {
                MaczkopatId = maczkopatId.Value
            };
            
            var result = Context.Mediator.Send<GetParcelsFromMaczkopatQuery, Task<List<OutpostImmobile.Core.Parcels.QueryResults.ParcelDto>>>(query).GetAwaiter().GetResult();
            return result.Count;
        }
        catch (Exception)
        {
            return 0;
        }
    }
    
    /// <summary>
    /// Get parcel status from database
    /// </summary>
    public string GetParcelStatus(string friendlyId)
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        var parcel = context.Parcels
            .AsNoTracking()
            .FirstOrDefault(p => p.FriendlyId == friendlyId);
        
        return parcel?.Status?.ToString() ?? "NotFound";
    }
    
    /// <summary>
    /// Check if parcel status matches expected
    /// </summary>
    public bool ParcelHasStatus(string friendlyId, string expectedStatus)
    {
        var actualStatus = GetParcelStatus(friendlyId);
        return actualStatus == expectedStatus;
    }
    
    /// <summary>
    /// Track parcel and get log entries count using TrackParcelByFriendlyIdQuery
    /// </summary>
    public int GetParcelLogsCount(string friendlyId)
    {
        try
        {
            var query = new TrackParcelByFriendlyIdQuery
            {
                FriendlyId = friendlyId
            };
            
            var result = Context.Mediator.Send<TrackParcelByFriendlyIdQuery, Task<IEnumerable<OutpostImmobile.Core.Parcels.QueryResults.ParcelLogDto>>>(query).GetAwaiter().GetResult();
            return result.Count();
        }
        catch (Exception)
        {
            return 0;
        }
    }
    
    private ParcelStatus ParseStatus(string status)
    {
        return status switch
        {
            "InWarehouse" => ParcelStatus.InWarehouse,
            "InTransit" => ParcelStatus.InTransit,
            "InMaczkopat" => ParcelStatus.InMaczkopat,
            "Delivered" => ParcelStatus.Delivered,
            "SendToStorage" => ParcelStatus.SendToStorage,
            "Forgotten" => ParcelStatus.Forgotten,
            "Expedited" => ParcelStatus.Expedited,
            "Deleted" => ParcelStatus.Deleted,
            "Sent" => ParcelStatus.Sent,
            "ToReturn" => ParcelStatus.ToReturn,
            _ => ParcelStatus.InWarehouse
        };
    }
}
