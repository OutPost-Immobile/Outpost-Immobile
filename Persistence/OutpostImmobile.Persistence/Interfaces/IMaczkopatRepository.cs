using OutpostImmobile.Persistence.Domain.Logs;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IMaczkopatRepository
{
    Task AddLogAsync(MaczkopatEventLogEntity log);
}