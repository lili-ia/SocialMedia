namespace SocialMedia.Application.DTOs.Block;

public class BlockedUserDto
{
    public Guid BlockedUserId { get; set; }

    public string BlockedUsername { get; set; } = null!;
    
    public DateTime BlockedAt { get; set; }
}