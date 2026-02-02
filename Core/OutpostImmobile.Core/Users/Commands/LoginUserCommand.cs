using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OutpostImmobile.Core.Mediator.Abstraction;
using OutpostImmobile.Core.Settings;
using OutpostImmobile.Persistence.Domain.Users;
using OutpostImmobile.Persistence.Exceptions;

namespace OutpostImmobile.Core.Users.Commands;

public record LoginUserCommand : IRequest<LoginUserCommand, Task<string>>
{
    public required string UserEmail { get; init; }
    public required string UserPassword { get; init; }
}

internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Task<string>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<JwtSettings>  _jwtSettings;
    public LoginUserCommandHandler(IServiceScopeFactory scopeFactory, IOptions<JwtSettings> jwtSettings)
    {
        _scopeFactory = scopeFactory;
        _jwtSettings = jwtSettings;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<UserInternal>>();
        
        var result = await signInManager.PasswordSignInAsync(request.UserEmail, request.UserPassword, false, false);
        
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid  login attempt");
        }
        
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserInternal>>();
        
        var user = await userManager.FindByEmailAsync(request.UserEmail);

        if (user is null)
        {
            throw new EntityNotFoundException("User not found");
        }
        
        var currentRoles = await userManager.GetRolesAsync(user);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, request.UserEmail),
            new Claim(ClaimTypes.Role, currentRoles.First())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}