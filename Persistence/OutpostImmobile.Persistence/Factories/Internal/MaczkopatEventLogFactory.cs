using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;

namespace OutpostImmobile.Persistence.Factories.Internal;

public class MaczkopatEventLogFactory : IEventLogFactory
{
    public Task<IEventLog> CreateEventLogAsync<TEventType>(CreateEventLogRequestBase<TEventType> request, CancellationToken ct) where TEventType : Enum
    {
        if (request is not CreateMaczkopatEventLogRequest maczkopatEventLogRequest)
        {
            throw new WrongEventLogTypeException("Wrong event log type");
        }

        var log = new MaczkopatEventLogEntity
        {
            MaczkopatId = maczkopatEventLogRequest.MaczkopatId,
            EventLogType = maczkopatEventLogRequest.EventLog,
            Message = request.Message ??  string.Empty,
        };

        return Task.FromResult<IEventLog>(log);
    }
}