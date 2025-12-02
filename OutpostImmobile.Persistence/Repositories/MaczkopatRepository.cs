using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class MaczkopatRepository : IMaczkopatRepository
{
    private readonly OutpostImmobileDbContext _context;

    public MaczkopatRepository(OutpostImmobileDbContext context)
    {
        _context = context;
    }

    public Task AddLogAsync(MaczkopatEventLogEntity log)
    {
        throw new NotImplementedException();
    }
}