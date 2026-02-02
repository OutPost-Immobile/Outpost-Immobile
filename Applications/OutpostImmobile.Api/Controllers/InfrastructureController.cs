using System.Net;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Infrastructure.Queries;
using OutpostImmobile.Core.Infrastructure.QueryResults;
using OutpostImmobile.Core.Mediator;

namespace OutpostImmobile.Api.Controllers;

public static class InfrastructureController
{
    public static IEndpointRouteBuilder MapInfrastructure(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/infrastructure");

        group.WithTags("Infrastructure");

        group.MapGet("/", GetAsync);
        group.MapGet("/enums/{enumName}", GetEnumTranslationsAsync);
        
        return routes;
    }

    private static async Task<TypedResponse<PingDto>> GetAsync([FromServices] IMediator mediator, CancellationToken ct)
    {
        var data = await mediator.Send(new InfrastructureQuery(), ct);

        return new TypedResponse<PingDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = data,
            Errors = null
        };
    }

    private static async Task<TypedResponse<List<EnumTranslationDto>>> GetEnumTranslationsAsync(
        [FromServices] IMediator mediator, 
        [FromRoute] string enumName,
        CancellationToken ct)
    {
        var data = await mediator.Send(new GetEnumTranslationsQuery
        {
            EnumName = enumName
        }, ct);

        return new TypedResponse<List<EnumTranslationDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Data = data,
            Errors = null
        };
    }
}