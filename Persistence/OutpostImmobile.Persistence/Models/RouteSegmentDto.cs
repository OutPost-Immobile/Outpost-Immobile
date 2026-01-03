using System.ComponentModel.DataAnnotations.Schema;

namespace OutpostImmobile.Persistence.Models;

public record RouteSegmentDto
{
    public required int Seq { get; init; }
    public required string GeoJson { get; init; }
    
    [Column(TypeName = "Segment_Dist")]
    public required double SegmentDist { get; init; }
    
    [Column(TypeName = "Total_Dist")]
    public required double TotalDist { get; init; }
}