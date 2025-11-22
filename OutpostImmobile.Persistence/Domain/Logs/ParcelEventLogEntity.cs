using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class ParcelEventLogEntity : AuditableEntity
{
    public Guid Id { get; set; }

    public required string Message { get; set; }
    public required ParcelEventLogType ParcelEventLogType { get; set; }
    
    public Guid ParcelId { get; set; }
    public ParcelEntity Parcel { get; set; }
}