using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OutpostImmobile.Api.Consts;
using OutpostImmobile.Api.Request;
using OutpostImmobile.Core.Mediator;
using OutpostImmobile.Core.Users.Commands;

namespace OutpostImmobile.Api.Controllers;

public static class UserController
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Users");

        group.MapPut("/Role", UpdateUserRoleAsync)
            .RequireAuthorization(PolicyNames.AdminOnly);
        
        group.MapPost("/Register", RegisterUserAsync);
        group.MapPost("/Login", LoginUserAsync);
        
        return routes;
    }
    
    private static async Task<Results<NoContent, BadRequest>> UpdateUserRoleAsync([FromServices] IMediator mediator, [FromBody] UpdateRoleRequest updateRoleRequest)
    {
        await mediator.Send(new UpdateUserRoleCommand
        {
            UserEmail = updateRoleRequest.Email,
            RoleName = updateRoleRequest.RoleName
        });
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<string>, BadRequest>> LoginUserAsync([FromServices] IMediator mediator, [FromBody] LoginRequest loginRequest)
    {
        var token = await mediator.Send(new LoginUserCommand
        {
            UserEmail = loginRequest.Email,
            UserPassword = loginRequest.Password
        });
        
        return TypedResults.Ok(token);
    }
    
    private static async Task<Results<NoContent, BadRequest>> RegisterUserAsync([FromServices] IMediator mediator,
        [FromBody] RegisterUserRequest registerUserRequest)
    {
        return TypedResults.NoContent();
    }
}