using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Interfaces;

namespace OutpostImmobile.Persistence.Factories.Internal;

public class MaczkopatEventLogFactory : IEventLogFactory
{
    public Task<IEventLog> CreateEventLogAsync()
    {
        throw new NotImplementedException();
    }
}