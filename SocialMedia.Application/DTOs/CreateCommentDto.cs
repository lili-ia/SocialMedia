using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public record CommentDTO([Required] string Text);