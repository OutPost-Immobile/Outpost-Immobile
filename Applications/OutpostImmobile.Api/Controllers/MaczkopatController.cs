using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Maczkopats.Commands;
using OutpostImmobile.Core.Maczkopats.Queries;
using OutpostImmobile.Core.Maczkopats.QueryResults;
using OutpostImmobile.Core.Mediator;

namespace OutpostImmobile.Api.Controllers;

public static class MaczkopatController
{
    public static IEndpointRouteBuilder MapMaczkopats(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/maczkopats");
        group.WithTags("Maczkopats");
        
        group.MapGet("/", GetAllMaczkopatsAsync)
            .RequireAuthorization(PolicyNames.AdminManager);
        
        group.MapGet("/{maczkopatId:Guid}", GetMaczkopatByIdAsync)
            .RequireAuthorization(PolicyNames.AdminManager);
        
        group.MapPost("/AddLog", AddLogAsync)
            .RequireAuthorization(PolicyNames.AdminManagerCourier);
        
        return routes;
    }
    
    private static async Task<TypedResponse<List<MaczkopatDto>>> GetAllMaczkopatsAsync([FromServices] IMediator mediator)
    {
        var maczkopats = await mediator.Send(new GetAllMaczkopatsQuery());

        return new TypedResponse<List<MaczkopatDto>>
        {
            Data = maczkopats,
            Errors = null,
            StatusCode = HttpStatusCode.OK
        };
    }
    
    private static async Task<TypedResponse<MaczkopatDetailsDto>> GetMaczkopatByIdAsync([FromServices] IMediator mediator, [FromRoute] Guid maczkopatId)
    {
        var maczkopat = await mediator.Send(new GetMaczkopatByIdQuery
        {
            MaczkopatId = maczkopatId
        });

        return new TypedResponse<MaczkopatDetailsDto>
        {
            Data = maczkopat,
            Errors = null,
            StatusCode = HttpStatusCode.OK
        };
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