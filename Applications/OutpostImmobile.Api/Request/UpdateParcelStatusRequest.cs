using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api.Request;

public record UpdateParcelStatusRequest
{
    public required string FriendlyId { get; init; }
    public required ParcelStatus ParcelStatus { get; init; }
}