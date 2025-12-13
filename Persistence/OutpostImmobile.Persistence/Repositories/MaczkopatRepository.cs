using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class MaczkopatRepository : IMaczkopatRepository
{
    private readonly OutpostImmobileDbContext _context;
    private readonly IEventLogFactory _eventLogFactory;

    public MaczkopatRepository(OutpostImmobileDbContext context, MaczkopatEventLogFactory eventLogFactory)
    {
        _context = context;
        _eventLogFactory = eventLogFactory;
    }

    public async Task AddLogAsync(Guid maczkopatId, MaczkopatEventLogType logType, CancellationToken ct)
    {
        var request = new CreateMaczkopatEventLogRequest
        {
            MaczkopatId = maczkopatId,
            EventLog = logType
        };

        var eventLogToAdd = await _eventLogFactory.CreateEventLogAsync(request, ct);

        if (eventLogToAdd is not MaczkopatEventLogEntity maczkopatEventLog)
        {
            throw new WrongEventLogTypeException("Created log is not of MaczkopatEventLog");
        }
        
        await _context.AddAsync(maczkopatEventLog, ct);
        await _context.SaveChangesAsync(ct);
    }
}