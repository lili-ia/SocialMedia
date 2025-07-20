namespace SocialMedia.Application.DTOs.User;

public class PublicUserProfileDto : UserProfileDto
{
    public int FollowersCount { get; set; }
    
    public int FolloweesCount { get; set; }
    
    public int PostsCount { get; set; }
}