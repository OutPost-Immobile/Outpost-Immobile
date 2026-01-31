using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Persistence.BusinessRules;

public class MaczkopatBusinessRules
{
    public static async ValueTask ThrowIfCannotUpdateMaczkopatState(OutpostImmobileDbContext context, ParcelEntity parcel, ParcelStatus status)
    {
        if (parcel.Status == status || status != ParcelStatus.InMaczkopat)
        {
            return;
        }

        var maczkopat = await context.Maczkopats
            .Where(x => x.Id == parcel.MaczkopatEntityId)
            .Select(x => new
            {
                x.Capacity,
                ParcelCount = x.Parcels.Count
            })
            .FirstAsync();

        var shouldUpdateParcelStatus = maczkopat.ParcelCount + 1 <= maczkopat.Capacity;
        
        if (!shouldUpdateParcelStatus)
        {
            throw new MaczkopatStateException("Could not update parcel status maczkopat is either full or not working");
        }
    }
}