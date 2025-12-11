using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class ParcelEventLogEntity : EventLogBase
{
    public required ParcelEventLogType ParcelEventLogType { get; init; }
    
    public Guid ParcelId { get; set; }
    public ParcelEntity Parcel { get; set; }
}