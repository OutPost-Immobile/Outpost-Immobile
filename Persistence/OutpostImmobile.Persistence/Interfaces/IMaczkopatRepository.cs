using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IMaczkopatRepository
{
    Task AddLogAsync(Guid maczkopatId, MaczkopatEventLogType logType, CancellationToken ct);
}