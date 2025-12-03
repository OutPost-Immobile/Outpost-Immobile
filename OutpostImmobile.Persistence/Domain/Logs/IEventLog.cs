namespace OutpostImmobile.Persistence.Domain.Logs;

public interface IEventLog
{
    public string? Message { get; set; }
}