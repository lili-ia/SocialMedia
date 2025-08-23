using System.ComponentModel.DataAnnotations;
using SocialMedia.Application.Attributes;
using SocialMedia.Shared.Attributes;

namespace SocialMedia.Shared.DTOs.User;

public record class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; init; } = null!;

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date)]
    [BirthDateInPast(ErrorMessage = "Birth date must be in the past.")]
    public DateTime BirthDate { get; init; }

    [StringLength(300, ErrorMessage = "Bio must be at most 300 characters.")]
    public string? Bio { get; init; }
}
