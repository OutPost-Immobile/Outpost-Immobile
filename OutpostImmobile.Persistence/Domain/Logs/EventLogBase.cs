using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain.Logs;

public abstract class EventLogBase : AuditableEntity, IEventLog
{
    public string? Message { get; set; }
}