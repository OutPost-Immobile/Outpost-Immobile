using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Helpers;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Parcels.Commands;
using OutpostImmobile.Core.Parcels.Queries;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Api.Controllers;

public static class ParcelController
{
    public static IEndpointRouteBuilder MapParcels(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Parcels");
        
        group.MapGet("/{parcelFriendlyId}/track", GetParcelLogsAsync);
        
        group.MapPost("/Update", UpdateParcelStatusAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        group.MapGet("Maczkopat/{maczkopatId:Guid}", GetParcelsFromMaczkopatAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        return routes;
    }
    
    private static async Task<TypedResponse<List<ParcelDto>>> GetParcelsFromMaczkopatAsync([FromServices] IMediator mediator,[FromRoute] Guid maczkopatId)
    {
        var parcels = await mediator.Send(new GetParcelsFromMaczkopatQuery
        {
            MaczkopatId = maczkopatId
        });

        return new TypedResponse<List<ParcelDto>>
        {
            Data = parcels,
            Errors = null,
            StatusCode = HttpStatusCode.OK
        };
    }
    
    private static async Task<Results<NoContent, BadRequest>> UpdateParcelStatusAsync([FromServices] IMediator mediator,[FromBody] List<UpdateParcelStatusRequest> requests)
    {
        await mediator.Send(new BulkUpdateParcelStatusCommand
        {
            ParcelModels = requests.Select(x => new BulkUpdateParcelStatusCommand.ParcelModel
            {
                FriendlyId = x.FriendlyId,
                Status = x.ParcelStatus
            })
        });
        return TypedResults.NoContent();
    }
    
    private static async Task<TypedResponse<IEnumerable<ParcelLogDto>>> GetParcelLogsAsync([FromServices] IMediator mediator, [FromRoute] string parcelFriendlyId)
    {
        var parcelEventLogs = await mediator.Send(new TrackParcelByFriendlyIdQuery
        {
            FriendlyId = parcelFriendlyId
        });

        return new TypedResponse<IEnumerable<ParcelLogDto>>
        {
            Data = parcelEventLogs,
            Errors = null,
            StatusCode = HttpStatusCode.OK
        };
    }
}