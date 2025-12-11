using System.ComponentModel.DataAnnotations.Schema;
using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain.Logs;

public abstract class EventLogBase : AuditableEntity, IEventLog
{
    public Guid Id { get; set; }
    [NotMapped]
    protected IEventLog? EventLog { get; init; }
    public required string Message { get; init; }
}