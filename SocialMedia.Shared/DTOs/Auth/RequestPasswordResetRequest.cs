using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Shared.DTOs.Auth;

public record class RequestPasswordResetRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; init; } = null!;
}