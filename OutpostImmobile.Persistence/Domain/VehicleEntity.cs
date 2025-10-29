using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class VehicleEntity : AuditableEntity
{
    public long Id { get; set; }
    public int Capacity { get; set; }

    public long DistanceRidden { get; set; }
    
    public ICollection<NumberTemplateEntity> NumberTemplateEntities { get; set; } = null!;
}