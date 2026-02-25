namespace SocialMedia.DTOs.Comment;

public class CreateCommentRequest
{
    public string Text { get; set; }
    
    public Guid ParentCommentId { get; set; }
}