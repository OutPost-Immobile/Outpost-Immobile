using System.Net;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Response;
using OutpostImmobile.Core.Interfaces;
using OutpostImmobile.Core.Models;

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

    private static async Task<TypedResponse<PingDto>> GetAsync([FromServices] IInfrastructureService service, CancellationToken ct)
    {
        var data = await service.PingAsync();

        return new TypedResponse<PingDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = data,
            Errors = null
        };
    }
}
