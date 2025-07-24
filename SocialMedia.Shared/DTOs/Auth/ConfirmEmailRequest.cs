using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Shared.DTOs.Auth;

public record class ConfirmEmailRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(150, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 and at most 150.")] 
    public string NewPassword { get; init; } = null!;
}