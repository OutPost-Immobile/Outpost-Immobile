using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Users.Commands;

public record UpdateUserRoleCommand : IRequest<UpdateUserRoleCommand, Task>
{
    public required string UserEmail { get; init; }
    public UserRoleName RoleName { get; init; }
}

internal class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Task>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UpdateUserRoleCommandHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(UpdateUserRoleCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserInternal>>();
        
        var userToUpdate = await userManager.FindByEmailAsync(request.UserEmail);

        if (userToUpdate == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        var currentRoles = await userManager.GetRolesAsync(userToUpdate);
        
        await userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
        
        var result = await userManager.AddToRoleAsync(userToUpdate, request.RoleName.ToString());

        if (!result.Succeeded)
        {
            var error = result.Errors.Select(x => x.Description);
            throw new Exception($"Failed to update user role: {error}");
        }
    }
}