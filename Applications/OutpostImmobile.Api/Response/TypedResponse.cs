using System.Net;

namespace OutpostImmobile.Api.Response;

public record TypedResponse<T>
{
    public required HttpStatusCode StatusCode { get; init; }
    public required T? Data { get; init; }
    public required string? Errors { get; init; }
}