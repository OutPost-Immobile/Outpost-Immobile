namespace OutpostImmobile.Core.Routes.QueryResult;

public record RouteDto
{
    public required long RouteId { get; init; }
    public required long Distance { get; init; }
}