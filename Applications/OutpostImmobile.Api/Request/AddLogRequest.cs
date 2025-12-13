using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api.Request;

public class AddLogRequest
{
    public Guid MaczkopatId { get; init; }
    public MaczkopatEventLogType LogType { get; init; }
}