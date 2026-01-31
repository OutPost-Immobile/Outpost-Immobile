using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;

namespace OutpostImmobile.Persistence.Factories.Internal;

public class CommunicationsEventLogFactory : IEventLogFactory
{
    public Task<EventLogBase> CreateEventLogAsync<TEventType>(CreateEventLogRequestBase<TEventType> request, CancellationToken ct) where TEventType : Enum
    {
        throw new NotImplementedException();
    }
}