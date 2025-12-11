using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class CommunicationEventLogRepository : ICommunicationEventLogRepository
{
    private readonly OutpostImmobileDbContext _context;
    private readonly IEventLogFactory _eventLogFactory;

    public CommunicationEventLogRepository(OutpostImmobileDbContext context, CommunicationsEventLogFactory evevEventLogFactory)
    {
        _context = context;
        _eventLogFactory = evevEventLogFactory;
    }

    public Task CreateLogAsync(CommunicationEventLogEntity log)
    {
        throw new NotImplementedException();
    }
}