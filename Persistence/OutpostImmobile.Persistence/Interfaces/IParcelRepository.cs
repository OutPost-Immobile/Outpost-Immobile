using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.Logs;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.Interfaces;

public interface IParcelRepository
{
    Task UpdateParcelStatusAsync(string friendlyId, ParcelStatus status);
    Task<List<ParcelEntity>> GetParcelsFromMaczkopatAsync(Guid maczkopatId);
    Task<IEnumerable<ParcelEventLogEntity>> GetParcelEventLogsAsync(string friendlyId);
}