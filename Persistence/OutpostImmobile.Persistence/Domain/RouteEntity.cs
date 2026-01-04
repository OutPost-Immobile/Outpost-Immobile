using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain.AuditableBase;
using OutpostImmobile.Persistence.Domain.Users;

namespace OutpostImmobile.Persistence.Domain;

public class RouteEntity : AuditableEntity
{
    public long Id { get; set; }

    public string StartAddressName { get; set; }
    public long StartAddressId { get; set; }

    public string EndAddressName { get; set; }
    public long EndAddressId { get; set; }
    
    public long Distance { get; set; }

    public ICollection<UserInternal> Couriers { get; set; } = [];
    public ICollection<VehicleEntity> AssignedVehicles { get; set; } = [];
    public ICollection<Point> Locations { get; set; } = [];
}

internal class RouteEntityConfiguration : IEntityTypeConfiguration<RouteEntity>
{
    public void Configure(EntityTypeBuilder<RouteEntity> builder)
    {
        builder.HasMany(x => x.Couriers)
            .WithOne(x => x.Route)
            .HasForeignKey(x => x.RouteId);
    }
}