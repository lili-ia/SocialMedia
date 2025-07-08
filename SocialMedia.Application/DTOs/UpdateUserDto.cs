using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record class UpdateUserDto
{
    [Required]
    public string Username { get; init; }
    
    [Required]
    public DateTime BirthDate { get; init; }
    
    public string? ProfilePicUrl { get; init; }
    
    public string? Bio { get; init; }
}
