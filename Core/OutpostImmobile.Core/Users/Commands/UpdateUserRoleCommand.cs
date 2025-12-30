using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Enums;
using OutpostImmobile.Persistence.Interfaces;

namespace OutpostImmobile.Core.Users.Commands;

public record UpdateUserRoleCommand : IRequest<UpdateUserRoleCommand, Task>
{
    public required string UserEmail { get; init; }
    public UserRoleName RoleName { get; init; }
}

internal class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Task>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserRoleCommand request, CancellationToken ct)
    {
        await _userRepository.UpdateUserRoleAsync(request.UserEmail, request.RoleName);
    }
}