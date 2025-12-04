using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Paralizator;
using OutpostImmobile.Core.Routes.QueryResult;

namespace OutpostImmobile.Api.Controllers;

public static class RouteController
{
    public static IEndpointRouteBuilder MapRouteController(this IEndpointRouteBuilder routes)
    {
        return routes;
    }

    private static async Task<TypedResponse<RouteDto>> GetRouteFromCourierAsync([FromServices] IMediator mediator,
        GetRouteFromCourierRequest request)
    {
        throw new NotImplementedException();
    }
}