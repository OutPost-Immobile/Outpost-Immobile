using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api.Request;

public class AddLogRequest
{
    public MaczkopatEventLogType LogType { get; set; }
}