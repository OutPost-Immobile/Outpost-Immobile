using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Interfaces;
using OutpostImmobile.Persistence.Repositories;

namespace OutpostImmobile.Core.Users.Commands;

public record LoginUserCommand : IRequest<LoginUserCommand, Task<UserInternal>>
{
    public required string UserEmail { get; init; }
    public required string UserPassword { get; init; }
}

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Task<UserInternal>>
{
    private readonly IUserRepository _userRepository;

    public LoginUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserInternal> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.LoginUserAsync(request.UserEmail, request.UserPassword);
    }
}