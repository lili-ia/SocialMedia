using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record CreateCommentDto([Required] string Text);