using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Factories.Interfaces;
using OutpostImmobile.Persistence.Factories.Internal;
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

    public Task AddLogAsync(MaczkopatEventLogEntity log)
    {
        throw new NotImplementedException();
    }
}