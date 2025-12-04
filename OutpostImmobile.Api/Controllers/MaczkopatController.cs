using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Paralizator;

namespace OutpostImmobile.Api.Controllers;

public static class MaczkopatController
{
    public static IEndpointRouteBuilder MapMaczkopats(this IEndpointRouteBuilder routes)
    {
        return routes;
    }

    private static async Task<Results<Created, BadRequest>> AddLogAsync([FromServices] IMediator mediator, AddLogRequest request)
    {
        throw new NotImplementedException();
    }
}