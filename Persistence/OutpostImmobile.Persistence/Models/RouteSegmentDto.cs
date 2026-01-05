using System.ComponentModel.DataAnnotations.Schema;

namespace OutpostImmobile.Persistence.Models;

public record RouteSegmentDto
{
    [Column("seq")]
    public required int Seq { get; init; }

    [Column("geojson")]
    public required string GeoJson { get; init; }
    
    [Column("segmentdist")]
    public required double SegmentDist { get; init; }

    [Column("totaldist")]
    public required double TotalDist { get; init; }
}