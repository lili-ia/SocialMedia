using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record UpdatePostDto([Required] string Text);