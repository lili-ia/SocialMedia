using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Shared.DTOs.Post;

public record class CreatePostRequest
{
    [StringLength(2000, ErrorMessage = "Post text must not exceed 500 characters")]
    public string? Text { get; set; }
}