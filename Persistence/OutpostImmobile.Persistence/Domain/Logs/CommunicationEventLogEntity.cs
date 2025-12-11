namespace OutpostImmobile.Persistence.Domain.Logs;

public class CommunicationEventLogEntity : EventLogBase
{
    public required string? Sender { get; init; }
    public required string? Receiver { get; init; }
    
    public Guid ParcelId { get; set; }
    public ParcelEntity Parcel { get; set; }
}