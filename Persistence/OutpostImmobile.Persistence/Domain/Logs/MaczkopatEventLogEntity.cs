using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class MaczkopatEventLogEntity : EventLogBase
{
    public required MaczkopatEventLogType EventLogType { get; init; }
    public Guid MaczkopatId { get; set; }
    public MaczkopatEntity Maczkopat { get; set; }
}