using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class MaczkopatEventLogEntity : AuditableEntity
{
    public Guid Id { get; set; }

    public required MaczkopatEventLogType EventLogType { get; set; }

    public required string? LogMessage { get; set; }

    public Guid MaczkopatId { get; set; }
    public MaczkopatEntity Maczkopat { get; set; }
}