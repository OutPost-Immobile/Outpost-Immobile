using System.Net;
using DispatchR;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Infrastructure.Queries;

namespace OutpostImmobile.Api.Controllers;

public static class InfrastructureController
{
    public static IEndpointRouteBuilder MapInfrastructure(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/infrastructure");

        group.WithTags("Infrastructure");

        group.MapGet("/", GetAsync);
        
        return routes;
    }

    private static async Task<TypedResponse<string>> GetAsync([FromServices] IMediator mediator, CancellationToken ct)
    {
        var data =  await mediator.Send(new InfrastructureQuery(), ct);

        return new TypedResponse<string>
        {
            StatusCode = HttpStatusCode.OK,
            Data = data.Message,
            Errors = null
        };
    }
}