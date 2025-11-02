namespace OutpostImmobile.Persistence.Domain;

public class LocationMarkerEntity
{
    public Guid Id { get; set; }

    public required int Longitude { get; set; }
    public required int Latitude { get; set; }

    public ICollection<RouteEntity> Routes { get; set; }
}