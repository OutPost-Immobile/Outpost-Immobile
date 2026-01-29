using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures;

/// <summary>
/// Setup fixture for creating test data (users, maczkopats, parcels).
/// Used in FitNesse SetUp pages to prepare the database before tests.
/// </summary>
public class SetupFixture : BaseFixture
{
    // Internal storage for values set via FitSharp Script methods
    private string? _maczkopatCode;
    private long _maczkopatCapacity;
    private string? _parcelFriendlyId;
    private string? _parcelProduct;
    private string? _parcelStatus;
    private string? _userEmail;
    private string? _userName;
    private string? _userRole;
    
    // Store created IDs
    private Guid _lastCreatedMaczkopatId;
    private Guid _lastCreatedUserId;
    private Guid _lastCreatedParcelId;
    
    #region FitSharp Setter Methods (for Script tables: | set X | value |)
    
    public void SetMaczkopatCode(string value) => _maczkopatCode = value;
    public void SetMaczkopatCapacity(long value) => _maczkopatCapacity = value;
    public void SetParcelFriendlyId(string value) => _parcelFriendlyId = value;
    public void SetParcelProduct(string value) => _parcelProduct = value;
    public void SetParcelStatus(string value) => _parcelStatus = value;
    public void SetUserEmail(string value) => _userEmail = value;
    public void SetUserName(string value) => _userName = value;
    public void SetUserRole(string value) => _userRole = value;
    
    #endregion
    
    /// <summary>
    /// Create a new maczkopat in the database
    /// </summary>
    public string CreateMaczkopat()
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        // Create area and address first
        var area = new AreaEntity
        {
            Id = DateTime.UtcNow.Ticks,
            AreaName = "Test Area"
        };
        context.Areas.Add(area);
        
        var address = new AddressEntity
        {
            Id = DateTime.UtcNow.Ticks + 1,
            City = "Warszawa",
            Street = "Testowa",
            PostalCode = "00-001",
            CountryCode = "PL",
            BuildingNumber = "1",
            Location = new Point(21.0, 52.0) { SRID = 4326 } // Dummy location for tests (ignored in test env)
        };
        context.Addresses.Add(address);
        context.SaveChanges();
        
        var maczkopat = new MaczkopatEntity
        {
            Id = Guid.NewGuid(),
            Code = _maczkopatCode ?? $"MACZ-{Guid.NewGuid().ToString()[..8]}",
            Capacity = _maczkopatCapacity > 0 ? _maczkopatCapacity : 10,
            AreaId = area.Id,
            AddressId = address.Id
        };
        
        context.Maczkopats.Add(maczkopat);
        context.SaveChanges();
        
        _lastCreatedMaczkopatId = maczkopat.Id;
        Context.CurrentMaczkopatId = maczkopat.Id;
        
        return maczkopat.Id.ToString();
    }
    
    /// <summary>
    /// Create a new external user in the database
    /// </summary>
    public string CreateExternalUser()
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        var user = new UserExternal
        {
            Id = Guid.NewGuid(),
            Email = _userEmail ?? $"test_{Guid.NewGuid()}@test.com",
            Name = _userName ?? "Test User",
            PhoneNumber = "+48123456789"
        };
        
        context.UsersExternal.Add(user);
        context.SaveChanges();
        
        _lastCreatedUserId = user.Id;
        Context.CurrentUserId = user.Id;
        
        return user.Id.ToString();
    }
    
    /// <summary>
    /// Create a parcel with specified status and assign to maczkopat
    /// </summary>
    public string CreateParcel()
    {
        using var context = Context.CreateContextAsync().GetAwaiter().GetResult();
        
        var maczkopatId = Context.CurrentMaczkopatId ?? _lastCreatedMaczkopatId;
        var receiverId = Context.CurrentUserId ?? _lastCreatedUserId;
        
        var status = _parcelStatus switch
        {
            "InWarehouse" => Persistence.Domain.StaticEnums.Enums.ParcelStatus.InWarehouse,
            "InTransit" => Persistence.Domain.StaticEnums.Enums.ParcelStatus.InTransit,
            "InMaczkopat" => Persistence.Domain.StaticEnums.Enums.ParcelStatus.InMaczkopat,
            "Delivered" => Persistence.Domain.StaticEnums.Enums.ParcelStatus.Delivered,
            "SendToStorage" => Persistence.Domain.StaticEnums.Enums.ParcelStatus.SendToStorage,
            _ => Persistence.Domain.StaticEnums.Enums.ParcelStatus.InWarehouse
        };
        
        var parcel = new ParcelEntity
        {
            Id = Guid.NewGuid(),
            FriendlyId = _parcelFriendlyId ?? $"PCK-{Guid.NewGuid().ToString()[..8]}",
            Product = _parcelProduct ?? "Test Product",
            Status = status,
            MaczkopatEntityId = maczkopatId,
            ReceiverUserExternalId = receiverId
        };
        
        context.Parcels.Add(parcel);
        context.SaveChanges();
        
        _lastCreatedParcelId = parcel.Id;
        
        return parcel.FriendlyId;
    }
    
    /// <summary>
    /// Get the last created maczkopat ID
    /// </summary>
    public string GetLastMaczkopatId()
    {
        return _lastCreatedMaczkopatId.ToString();
    }
    
    /// <summary>
    /// Get the last created user ID
    /// </summary>
    public string GetLastUserId()
    {
        return _lastCreatedUserId.ToString();
    }
    
    /// <summary>
    /// Set the current maczkopat context by ID
    /// </summary>
    public void SetCurrentMaczkopat(string maczkopatId)
    {
        if (Guid.TryParse(maczkopatId, out var id))
        {
            Context.CurrentMaczkopatId = id;
            _lastCreatedMaczkopatId = id;
        }
    }
    
    /// <summary>
    /// Set the current user context by ID
    /// </summary>
    public void SetCurrentUser(string userId)
    {
        if (Guid.TryParse(userId, out var id))
        {
            Context.CurrentUserId = id;
            _lastCreatedUserId = id;
        }
    }
    
    /// <summary>
    /// Simulate user login (for authorization scenarios)
    /// </summary>
    public bool LoginAs(string role)
    {
        Context.IsAuthenticated = true;
        Context.CurrentUserRole = role;
        return true;
    }
    
    /// <summary>
    /// Simulate user logout
    /// </summary>
    public void Logout()
    {
        Context.IsAuthenticated = false;
        Context.CurrentUserRole = null;
    }
    
    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    public bool IsUserAuthenticated()
    {
        return Context.IsAuthenticated;
    }
    
    /// <summary>
    /// Get current user role
    /// </summary>
    public string? GetCurrentUserRole()
    {
        return Context.CurrentUserRole;
    }
}
