using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record GetUserInfoRequest([EmailAddress] [Required] string UserEmail);