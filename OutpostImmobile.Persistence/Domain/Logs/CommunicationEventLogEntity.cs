namespace OutpostImmobile.Persistence.Domain.Logs;

public class CommunicationEventLogEntity : EventLogBase
{
    public Guid Id { get; set; }

    public required string? Sender { get; set; }
    public required string? Receiver { get; set; }
    public required string? Message { get; set; }
    
    public Guid ParcelId { get; set; }
    public ParcelEntity Parcel { get; set; }
}