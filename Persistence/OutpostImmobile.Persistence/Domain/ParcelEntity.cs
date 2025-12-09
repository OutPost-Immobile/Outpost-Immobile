using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Domain;

public class ParcelEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string FriendlyId { get; set; }
    public required string Product { get; set; }
    public ParcelStatus? Status { get; set; }
    
    public Guid? FromUserExternalId { get; set; }
    
    public Guid? ReceiverUserExternalId { get; set; }
    
    public Guid MaczkopatEntityId { get; set; }
    public MaczkopatEntity Maczkopat { get; set; }
    
    public ICollection<ParcelEventLogEntity> ParcelEventLogs { get; set; }
    public ICollection<CommunicationEventLogEntity> CommunicationEventLogs { get; set; }
}

internal class ParcelEntityConfiguration :  IEntityTypeConfiguration<ParcelEntity>
{
    public void Configure(EntityTypeBuilder<ParcelEntity> builder)
    {
        builder
            .HasOne(x => x.Maczkopat)
            .WithMany(x => x.Parcels)
            .HasForeignKey(x => x.MaczkopatEntityId);
    }
}