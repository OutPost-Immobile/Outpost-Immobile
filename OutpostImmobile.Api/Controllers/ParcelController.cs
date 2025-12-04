using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Paralizator;
using OutpostImmobile.Core.Parcels.QueryResults;

namespace OutpostImmobile.Api.Controllers;

public static class ParcelController
{
    public static IEndpointRouteBuilder MapParcels(this IEndpointRouteBuilder routes)
    {
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
}