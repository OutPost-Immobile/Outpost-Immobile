using NetTopologySuite.Geometries;
using OutpostImmobile.Persistence.Domain.AuditableBase;

namespace OutpostImmobile.Persistence.Domain;

public class RouteEntity : AuditableEntity
{
    public long Id { get; set; }

    public long StartAddressId { get; set; }
    public long EndAddressId { get; set; }
    public long Distace { get; set; }

    public ICollection<VehicleEntity> AssignedVehicles { get; set; } = null!;
    public ICollection<Point> Locations { get; set; }
}