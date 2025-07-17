using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs.Auth;

public record class RefreshTokenDto
{
    [Required(ErrorMessage = "Token is required.")] 
    public string Token { get; init; } = null!;
}