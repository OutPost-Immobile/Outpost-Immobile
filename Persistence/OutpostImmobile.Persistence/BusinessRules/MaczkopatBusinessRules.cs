using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.BusinessRules;

public class MaczkopatBusinessRules
{
    public static async ValueTask<bool> ShouldUpdateMaczkopatState(OutpostImmobileDbContext context, ParcelEntity parcel, ParcelStatus status)
    {
        if (parcel.Status == status)
        {
            return false;
        }

        var maczkopat = await context.Maczkopats
            .Where(x => x.Id == parcel.MaczkopatEntityId)
            .Select(x => new
            {
                x.Capacity,
                ParcelCount = x.Parcels.Count
            })
            .FirstAsync();

        return maczkopat.ParcelCount + 1 <= maczkopat.Capacity;
    }
}