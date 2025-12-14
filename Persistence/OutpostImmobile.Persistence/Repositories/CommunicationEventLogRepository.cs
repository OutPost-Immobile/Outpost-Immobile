using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class CommunicationEventLogRepository : ICommunicationEventLogRepository
{
    private readonly IDbContextFactory<OutpostImmobileDbContext> _dbContextFactory;
    private readonly IEventLogFactory _eventLogFactory;

    public CommunicationEventLogRepository(CommunicationsEventLogFactory evevEventLogFactory, IDbContextFactory<OutpostImmobileDbContext> dbContextFactory)
    {
        _eventLogFactory = evevEventLogFactory;
        _dbContextFactory = dbContextFactory;
    }

    public Task CreateLogAsync(CommunicationEventLogEntity log)
    {
        throw new NotImplementedException();
    }
}