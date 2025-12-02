using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IParcelRepository
{
    Task UpdateParcelStatusAsync(Guid parcelId, ParcelStatus status);
    Task<ParcelEntity> GetParcelsFromMaczkopatAsync(Guid maczkopatId);
}