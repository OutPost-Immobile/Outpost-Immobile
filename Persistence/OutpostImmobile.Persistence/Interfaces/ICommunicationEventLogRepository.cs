using OutpostImmobile.Persistence.Domain.Logs;

namespace OutpostImmobile.Persistence.Interfaces;

public interface ICommunicationEventLogRepository
{
    Task CreateLogAsync(CommunicationEventLogEntity log);
}