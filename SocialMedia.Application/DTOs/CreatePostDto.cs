using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record CreatePostDto([Required] string Text);