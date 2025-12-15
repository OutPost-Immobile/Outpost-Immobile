using Microsoft.EntityFrameworkCore;
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
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IEventLogFactory _eventLogFactory;

    public MaczkopatRepository(MaczkopatEventLogFactory eventLogFactory, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _eventLogFactory = eventLogFactory;
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddLogAsync(Guid maczkopatId, MaczkopatEventLogType logType, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        var request = new CreateMaczkopatEventLogRequest
        {
            MaczkopatId = maczkopatId,
            EventLog = logType,
            Message = null
        };

        var eventLogToAdd = await _eventLogFactory.CreateEventLogAsync(request, ct);

        if (eventLogToAdd is not MaczkopatEventLogEntity maczkopatEventLog)
        {
            throw new WrongEventLogTypeException("Created log is not of MaczkopatEventLog");
        }
        
        await context.AddAsync(maczkopatEventLog, ct);
        await context.SaveChangesAsync(ct);
    }
}