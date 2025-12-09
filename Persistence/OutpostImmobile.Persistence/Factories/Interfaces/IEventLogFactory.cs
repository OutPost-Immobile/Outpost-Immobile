using OutpostImmobile.Persistence.Domain.Logs;

namespace OutpostImmobile.Persistence.Factories.Interfaces;

public interface IEventLogFactory
{
    Task<IEventLog> CreateEventLogAsync();
}