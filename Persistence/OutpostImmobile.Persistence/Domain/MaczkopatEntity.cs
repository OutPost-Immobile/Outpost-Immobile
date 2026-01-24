using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.Logs;

namespace OutpostImmobile.Persistence.Domain;

public class MaczkopatEntity : AuditableEntity
{
    public Guid Id { get; set; }

    public required string Code { get; set; }
    
    public required long Capacity { get; set; }
    
    public long AreaId { get; set; }
    public AreaEntity Area { get; set; }
    
    public long AddressId { get; set; }
    public AddressEntity Address { get; set; }
    
    public ICollection<ParcelEntity> Parcels { get; set; } = null!;
    public ICollection<MaczkopatEventLogEntity> MaczkopatEventLogs { get; set; } = null!;
}