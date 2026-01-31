using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Request;

namespace OutpostImmobile.Persistence.Factories.Interfaces;

public interface IEventLogFactory
{
    Task<EventLogBase> CreateEventLogAsync<TEventType>(CreateEventLogRequestBase<TEventType> request, CancellationToken ct = default) where TEventType : Enum;
}