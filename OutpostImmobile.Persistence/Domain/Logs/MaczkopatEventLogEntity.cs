using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class MaczkopatEventLogEntity : EventLogBase
{
    public Guid Id { get; set; }

    public required MaczkopatEventLogType EventLogType { get; set; }

    public required string? Message { get; set; }

    public Guid MaczkopatId { get; set; }
    public MaczkopatEntity Maczkopat { get; set; }
}