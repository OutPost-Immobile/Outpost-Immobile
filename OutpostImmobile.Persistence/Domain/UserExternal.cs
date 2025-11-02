using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class UserExternal : AuditableEntity
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }

    public Guid? InternalUserId { get; set; }
    public UserInternal? UserInternal { get; set; }
    
    public ICollection<ParcelEntity> Parcels { get; set; } = null!;
}