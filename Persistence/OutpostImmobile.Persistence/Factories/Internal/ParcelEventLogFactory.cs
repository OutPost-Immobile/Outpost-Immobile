using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;

namespace OutpostImmobile.Persistence.Factories.Internal;

public class ParcelEventLogFactory : IEventLogFactory
{
    public Task<IEventLog> CreateEventLogAsync<TEventType>(CreateEventLogRequestBase<TEventType> request, CancellationToken ct) where TEventType : Enum
    {
        if (request is not CreateParcelEventLogTypeRequest createParcelEventLogTypeRequest)
        {
            throw new WrongEventLogTypeException("Wrong event log type");
        }

        var log = new ParcelEventLogEntity
        {
            ParcelId = createParcelEventLogTypeRequest.ParcelId,
            Message = request.Message ?? string.Empty,
            ParcelEventLogType = ParcelEventLogType.StatusChange
        };
        
        return Task.FromResult<IEventLog>(log);
    }
}