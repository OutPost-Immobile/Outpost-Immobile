namespace OutpostImmobile.Api.Request;

public record SaveCalculatedDistanceRequest
{
    public required long RouteId { get; init; }
    public required long CalculatedDistance { get; init; }
}