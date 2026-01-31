using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class EventLogBase : AuditableEntity
{
    public Guid Id { get; set; }
    public required string Message { get; init; }
}