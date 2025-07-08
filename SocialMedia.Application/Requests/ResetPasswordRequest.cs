using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.Requests;

public record class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    public string Token { get; init; } = null!;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string NewPassword { get; init; } = null!;
}