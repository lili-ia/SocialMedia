using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record UpdatePostDto([Required] int PostId, [Required] string Text);