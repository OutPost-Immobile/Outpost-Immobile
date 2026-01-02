using System.ComponentModel.DataAnnotations;

namespace OutpostImmobile.Core.Settings;

public record JwtSettings
{
    [Required]
    public required string Issuer { get; init; }
    
    [Required]
    public required string Audience { get; init; }
    
    [Required]
    public required string Key { get; init; }
}