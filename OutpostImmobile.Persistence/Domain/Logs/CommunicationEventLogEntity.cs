using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain.Logs;

public class CommunicationEventLogEntity : AuditableEntity
{
    public Guid Id { get; set; }

    public required string? Sender { get; set; }
    public required string? Receiver { get; set; }
    public required string Message { get; set; }

    public Guid ParcelId { get; set; }
    public ParcelEntity Parcel { get; set; }
}