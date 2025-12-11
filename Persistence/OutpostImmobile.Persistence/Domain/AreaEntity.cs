namespace OutpostImmobile.Persistence.Domain;

public class AreaEntity
{
    public long Id { get; set; }

    public required string AreaName { get; set; }

    public ICollection<MaczkopatEntity> Maczkopats { get; set; } = null!;
}