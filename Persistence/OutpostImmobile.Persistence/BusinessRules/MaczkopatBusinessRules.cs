using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.BusinessRules;

public class MaczkopatBusinessRules
{
    public static async ValueTask<bool> ShouldUpdateMaczkopatState(OutpostImmobileDbContext context, ParcelEntity parcel, ParcelStatus status)
    {
        if (parcel.Status == status || (status != ParcelStatus.InMaczkopat && status != ParcelStatus.Delivered))
        {
            return false;
        }

        var maczkopat = await context.Maczkopats
            .Where(x => x.Id == parcel.MaczkopatEntityId)
            .FirstAsync();

        switch (status)
        {
            case ParcelStatus.Delivered:
                maczkopat.Capacity -= 1;
                break;
            
            case ParcelStatus.InMaczkopat:
            {
                var capacity = maczkopat.Capacity + 1;

                if (capacity > maczkopat.Capacity)
                {
                    return false;
                }
            
                maczkopat.Capacity += 1;
                break;
            }
        }

        await context.SaveChangesAsync();

        return true;
    }
}