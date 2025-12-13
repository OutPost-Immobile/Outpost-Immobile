using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Factories.Request;

public abstract record CreateEventLogRequestBase<TEventLogType> where TEventLogType : Enum
{
    public required TEventLogType EventLog { get; init; }
}

public record CreateMaczkopatEventLogRequest : CreateEventLogRequestBase<MaczkopatEventLogType>
{
    public required Guid MaczkopatId { get; init; }
}

public record CreateParcelEventLogTypeRequest : CreateEventLogRequestBase<ParcelEventLogType>
{
    public required Guid ParcelId { get; init; }
}

public record CreateCommunicationEventLogType : CreateEventLogRequestBase<CommunicationEventLogType>
{
    public required string Sender { get; init; }   
    public required string Receiver { get; init; }   
    public required string Message { get; init; }   
}