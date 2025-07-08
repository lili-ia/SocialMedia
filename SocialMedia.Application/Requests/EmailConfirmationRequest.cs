using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.Requests;

public record class EmailConfirmationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;
}