using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class MaczkopatEntity : AuditableEntity
{
    public Guid Id { get; set; }

    public required string FriendlyId { get; set; }
    
    public long AreaId { get; set; }
    public AreaEntity Area { get; set; }
    
    public int AddressId { get; set; }
    public AddressEntity Address { get; set; }
    
    public ICollection<ParcelEntity> Parcels { get; set; } = null!;
}