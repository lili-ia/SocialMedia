using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs.Comment;

public record class CreateCommentRequest
{
    [Required(ErrorMessage = "Comment text is required.")]
    [MinLength(1, ErrorMessage = "Comment must not be empty.")]
    [MaxLength(500, ErrorMessage = "Comment must not exceed 500 characters.")]
    public string Text { get; set; } = null!;
}