using System.Net;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Helpers;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Routes.Queries;
using OutpostImmobile.Core.Routes.QueryResult;

namespace OutpostImmobile.Api.Controllers;

public static class RouteController
{
    public static IEndpointRouteBuilder MapRouteController(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/routes");

        group.MapGet("/{courierId:Guid}", GetRouteFromCourierAsync);
        
        return routes;
    }

    private static async Task<TypedResponse<List<RouteDto>>> GetRouteFromCourierAsync([FromServices] IMediator mediator, [FromRoute] Guid courierId)
    {
        try
        {
            var routes = await mediator.Send(new GetRoutesFromCourierQuery
            {
                CourierId = courierId
            });

            return new TypedResponse<List<RouteDto>>
            {
                StatusCode = HttpStatusCode.OK,
                Data = routes,
                Errors = null
            };
        }
        catch (Exception e)
        {
            return ExceptionHelper.HandleErrors<List<RouteDto>>([], e.Message);
        }
    }
}