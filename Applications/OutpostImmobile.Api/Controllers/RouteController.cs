using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Helpers;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Routes.Queries;
using OutpostImmobile.Core.Routes.QueryResult;
using OutpostImmobile.Persistence.Models;

namespace OutpostImmobile.Api.Controllers;

public static class RouteController
{
    public static IEndpointRouteBuilder MapRoute(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/routes");

        group.MapGet("/{courierId:Guid}", GetRouteFromCourierAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        group.MapGet("/geojson-stream/{routeId:long}", GetRouteGeoJsonAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        return routes;
    }
    
    private static async Task<TypedResponse<List<RouteDto>>> GetRouteFromCourierAsync([FromServices] IMediator mediator, [FromRoute] Guid courierId)
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

    private static async Task<IAsyncEnumerable<RouteSegmentDto>> GetRouteGeoJsonAsync([FromServices] IMediator mediator, long routeId)
    {
        var result = await mediator.Send(new GetRouteGeoJsonQuery
        {
            RouteId = routeId
        });

        return result;
    }
}