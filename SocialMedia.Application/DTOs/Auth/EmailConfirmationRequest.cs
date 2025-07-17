using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs.Auth;

public record class EmailConfirmationRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; init; } = null!;
}