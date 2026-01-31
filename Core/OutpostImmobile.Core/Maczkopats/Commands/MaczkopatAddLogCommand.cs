using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Request;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Maczkopats.Commands;

public record MaczkopatAddLogCommand : IRequest<MaczkopatAddLogCommand, Task>
{
    public required Guid MaczkopatId { get; init; }
    public required MaczkopatEventLogType LogType { get; init; }
}

internal class MaczkopatAddLogCommandHandler : IRequestHandler<MaczkopatAddLogCommand, Task>
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IEventLogFactory _eventLogFactory;

    public MaczkopatAddLogCommandHandler([FromKeyedServices("Maczkopat")]IEventLogFactory eventLogFactory, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _eventLogFactory = eventLogFactory;
        _dbContextFactory = dbContextFactory;
    }

    public async Task Handle(MaczkopatAddLogCommand request, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        
        var logRequest = new CreateMaczkopatEventLogRequest
        {
            MaczkopatId = request.MaczkopatId,
            EventLog = request.LogType,
            Message = null
        };

        var eventLogToAdd = await _eventLogFactory.CreateEventLogAsync(logRequest, ct);

        if (eventLogToAdd is not MaczkopatEventLogEntity maczkopatEventLog)
        {
            throw new WrongEventLogTypeException("Created log is not of MaczkopatEventLog");
        }
        
        await context.AddAsync(maczkopatEventLog, ct);
        await context.SaveChangesAsync(ct);    
    }
}