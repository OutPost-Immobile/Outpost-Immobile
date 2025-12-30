using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Maczkopats.Commands;
using OutpostImmobile.Core.Mediator;

namespace OutpostImmobile.Api.Controllers;

public static class MaczkopatController
{
    public static IEndpointRouteBuilder MapMaczkopats(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/maczkopats/");
        group.WithTags("Maczkopats");
        
        group.MapPost("/AddLog", AddLogAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        return routes;
    }
    
    private static async Task<Results<Created, BadRequest>> AddLogAsync([FromServices] IMediator mediator, AddLogRequest request)
    {
        await mediator.Send(new MaczkopatAddLogCommand
        {
            LogType = request.LogType,
            MaczkopatId = request.MaczkopatId
        });

        return TypedResults.Created();
    }
}