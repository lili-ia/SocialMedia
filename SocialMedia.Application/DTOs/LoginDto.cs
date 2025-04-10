using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record LoginDto([EmailAddress] string Email, string RawPassword);