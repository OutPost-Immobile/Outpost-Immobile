using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Domain.AuditableBase;

public abstract class AuditableEntity
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public UserInternal? CreatedBy { get; set; }
    public Guid? CreatedById { get; set; }
}