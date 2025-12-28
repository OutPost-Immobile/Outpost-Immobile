using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Persistence.Domain.StaticEnums.Enums;

namespace OutpostImmobile.Api.Controllers;

public static class UserController
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Users");

        group.MapPost("/Role", UpdateUserRoleAsync);
        
        return routes;
    }

    private static async Task<Results<NoContent, BadRequest>> UpdateUserRoleAsync([FromServices] IMediator mediator,
        [FromBody] UserRoles userRoles)
    {
        return TypedResults.NoContent();
    }
}