using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record UpdateUserDto([Required] string Username, DateTime? BirthDate, string? ProfilePicUrl, string? Bio);
