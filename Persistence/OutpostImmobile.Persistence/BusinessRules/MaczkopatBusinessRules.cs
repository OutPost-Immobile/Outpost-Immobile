using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Persistence.Domain;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Persistence.BusinessRules;

public class MaczkopatBusinessRules
{
    public static async ValueTask<bool> ShouldUpdateMaczkopatState(OutpostImmobileDbContext context, ParcelEntity parcel, ParcelStatus status)
    {
        // Jeśli status się nie zmienia, nie aktualizujemy
        if (parcel.Status == status)
        {
            return false;
        }

        // Przy wydawaniu paczki (zmiana z InMaczkopat na inny status) - zawsze dozwolone
        if (parcel.Status == ParcelStatus.InMaczkopat && status != ParcelStatus.InMaczkopat)
        {
            return true;
        }

        // Przy przyjmowaniu paczki do maczkopatu - sprawdź pojemność
        if (status == ParcelStatus.InMaczkopat)
        {
            var maczkopat = await context.Maczkopats
                .Where(x => x.Id == parcel.MaczkopatEntityId)
                .Select(x => new
                {
                    x.Capacity,
                    ParcelCount = x.Parcels.Count(p => p.Status == ParcelStatus.InMaczkopat)
                })
                .FirstAsync();

            return maczkopat.ParcelCount + 1 <= maczkopat.Capacity;
        }

        // Inne zmiany statusu są dozwolone
        return true;
    }
}