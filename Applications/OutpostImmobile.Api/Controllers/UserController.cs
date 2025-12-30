using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Users.Commands;

namespace OutpostImmobile.Api.Controllers;

public static class UserController
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Users");

        group.MapPut("/Role", UpdateUserRoleAsync);

        group.MapPost("/Register", RegisterUserAsync);
        
        return routes;
    }
    [Authorize(Roles = "Admin")]
    private static async Task<Results<NoContent, BadRequest>> UpdateUserRoleAsync([FromServices] IMediator mediator,
        [FromBody] UpdateRoleRequest updateRoleRequest)
    {
        await mediator.Send(new UpdateUserRoleCommand
        {
            UserEmail = updateRoleRequest.Email,
            RoleName = updateRoleRequest.RoleName
        });
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest>> RegisterUserAsync([FromServices] IMediator mediator,
        RegisterUserRequest registerUserRequest)
    {
        return TypedResults.NoContent();
    }
}