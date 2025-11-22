namespace OutpostImmobile.Persistence.Domain.Routes;

public class LocationMarkerEntity
{
    public Guid Id { get; set; }

    public required double Longitude { get; set; }
    public required double Latitude { get; set; }

    public ICollection<RouteEntity> Routes { get; set; }
}