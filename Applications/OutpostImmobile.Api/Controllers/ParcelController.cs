using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Parcels.Queries;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Api.Controllers;

public static class ParcelController
{
    public static IEndpointRouteBuilder MapParcels(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Parcels");
        
        group.MapGet("/{parcelFriendlyId}/track", GetParcelLogsAsync);
        
        return routes;
    }

    private static async Task<TypedResponse<List<ParcelDto>>> GetParcelsFromMaczkopatAsync(
        [FromServices] IMediator mediator, GetParcelsFromMaczkopatRequest request)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<NoContent, BadRequest>> UpdateParcelStatusAsync([FromServices] IMediator mediator,
        UpdateParcelStatusRequest request)
    {
        throw new NotImplementedException();
    }
    
    private static async Task<Results<Ok<IEnumerable<ParcelLogDto>>, BadRequest>> GetParcelLogsAsync([FromServices] IMediator mediator,
        [FromRoute] string parcelFriendlyId)
    {
        var parcelEventLogs = await mediator.Send(new TrackParcelByFriendlyIdQuery
        {
            FriendlyId = parcelFriendlyId
        });
        
        return TypedResults.Ok(parcelEventLogs);
    }
}