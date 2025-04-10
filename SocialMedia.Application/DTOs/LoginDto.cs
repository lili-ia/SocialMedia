using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record LoginDto([EmailAddress] [Required] string Email, [Required] string RawPassword);