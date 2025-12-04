using Microsoft.EntityFrameworkCore;
using OutpostImmobile.Core.Maczkopats.Commands;
using OutpostImmobile.Core.Paralizator;
using OutpostImmobile.Core.Parcels.Commands;
using OutpostImmobile.Core.Parcels.Queries;
using OutpostImmobile.Core.Routes.Queries;
using OutpostImmobile.Persistence;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api;

public class Tester
{
    public static async Task TestUseCases(IMediator mediator, OutpostImmobileDbContext context)
    {
        //PU8 odbior paczek z maczkopatu
        var targetMaczkopatId = await context.Maczkopats
            .Select(x => x.Id)
            .FirstAsync();

        var parcelsFromTargetMaczkopat = await mediator.Send(new GetParcelsFromMaczkopatQuery
        {
            MaczkopatId = targetMaczkopatId
        });
        
        var parcelToDeliver = parcelsFromTargetMaczkopat.First();

        if (parcelToDeliver.Status == ParcelStatus.InMaczkopat)
        {
            parcelToDeliver.Status = ParcelStatus.Delivered;   
        }

        await mediator.Send(new UpdateParcelStatusCommand
        {
            ParcelId = parcelToDeliver.ParcelId,
            Status = parcelToDeliver.Status,
        });

        await mediator.Send(new MaczkopatAddLogCommand
        {
            LogType = MaczkopatEventLogType.LockerOpened
        });
        
        //PU7 odbior paczek z magazynu
        var targetMaczkopatId2 = await context.Maczkopats
            .Select(x => x.Id)
            .FirstAsync();

        var courierId = Guid.NewGuid();

        var courierRute = await mediator.Send(new GetRoutesFromCourierQuery
        {
            CourierId = courierId
        });
        
        var parcelsFromCourier = await mediator.Send(new GetParcelsFromMaczkopatQuery
        {
            MaczkopatId = targetMaczkopatId
        });

        foreach (var parcel in parcelsFromCourier)
        {
            parcel.Status = ParcelStatus.InTransit;

            await mediator.Send(new UpdateParcelStatusCommand
            {
                ParcelId = parcel.ParcelId,
                Status = parcel.Status
            });
            
            if (parcel.Status == ParcelStatus.InMaczkopat)
            {
                parcel.Status = ParcelStatus.Delivered;   
            }

            await mediator.Send(new UpdateParcelStatusCommand
            {
                ParcelId = parcel.ParcelId,
                Status = parcel.Status,
            });

            await mediator.Send(new MaczkopatAddLogCommand
            {
                LogType = MaczkopatEventLogType.LockerOpened
            });

            parcelsFromCourier.Remove(parcel);
            courierRute.RemoveAt(courierRute.Count - 1);
        }
    }
}