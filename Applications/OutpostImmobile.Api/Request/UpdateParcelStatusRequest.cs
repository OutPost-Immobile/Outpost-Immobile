using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api.Request;

public class UpdateParcelStatusRequest
{
    public ParcelStatus ParcelStatus { get; set; }
}