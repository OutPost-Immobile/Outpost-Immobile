using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.Logs;

namespace OutpostImmobile.Persistence.Domain;

public class ParcelEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string FriendlyId { get; set; }
    
    public Guid? FromUserExternalId { get; set; }
    
    public Guid? ReceiverUserExternalId { get; set; }
    
    public ICollection<ParcelEventLogEntity> ParcelEventLogs { get; set; }
    public ICollection<CommunicationEventLogEntity> CommunicationEventLogs { get; set; }
}