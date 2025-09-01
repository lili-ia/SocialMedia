namespace SocialMedia.Application.DTOs.User;

public class UserPublicDto
{
    public string Username { get; set; } = null!;

    public string? ProfilePicUrl { get; set; }
    
    public string? Bio { get; set; }
    
    public int PostsCount { get; set; } 
    
    public int FollowersCount { get; set; } 
    
    public int FolloweesCount { get; set; } 
}