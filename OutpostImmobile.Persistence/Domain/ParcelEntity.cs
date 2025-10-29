using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class ParcelEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string FriendlyId { get; set; }
    
    public Guid? FromUserId { get; set; }
    public UserExternal? FromUser { get; set; }
    
    public Guid? ReceiverUserId { get; set; }
    public UserExternal? ReceiverUser { get; set; }
}