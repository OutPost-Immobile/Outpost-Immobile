using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Persistence.Repositories;

public class ParcelRepository : IParcelRepository
{
    private readonly OutpostImmobileDbContext _context;

    public ParcelRepository(OutpostImmobileDbContext context)
    {
        _context = context;
    }

    public Task UpdateParcelStatusAsync(Guid parcelId, ParcelStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<ParcelEntity> GetParcelsFromMaczkopatAsync(Guid maczkopatId)
    {
        throw new NotImplementedException();
    }
}