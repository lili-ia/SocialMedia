using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record class RefreshTokenDto
{
    [Required] 
    public string Token { get; init; } = null!;
}