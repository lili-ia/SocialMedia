using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.DTOs.Block;

public class BlockedUserDto : UserPreviewDto
{
    public DateTime BlockedAt { get; set; }
}