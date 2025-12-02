using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class CommunicationEventLogRepository : ICommunicationEventLogRepository
{
    private readonly OutpostImmobileDbContext _context;

    public CommunicationEventLogRepository(OutpostImmobileDbContext context)
    {
        _context = context;
    }

    public Task CreateLogAsync(CommunicationEventLogEntity log)
    {
        throw new NotImplementedException();
    }
}