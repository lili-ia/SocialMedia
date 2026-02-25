namespace SocialMedia.Application.DTOs.Block;

public class BlockResponse
{
    public Guid BlockerId { get; set; }
    
    public Guid BlockedId { get; set; }
    
    public DateTime BlockedAt { get; set; }
}