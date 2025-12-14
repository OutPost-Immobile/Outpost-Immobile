using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Domain;

public class VehicleEntity : AuditableEntity
{
    public long Id { get; set; }
    public int Capacity { get; set; }

    public long DistanceRidden { get; set; }
    
    public Guid DriverId { get; set; }
    public UserInternal Driver { get; set; }
    
    public ICollection<NumberTemplateEntity> NumberTemplateEntities { get; set; } = null!;
}

internal class VehicleEntityConfiguration : IEntityTypeConfiguration<VehicleEntity>
{
    public void Configure(EntityTypeBuilder<VehicleEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Driver)
            .WithMany()
            .HasForeignKey(x => x.DriverId);
    }
}