namespace SocialMedia.Application.DTOs.Follow;

public class FollowResponse
{
    public bool IsFollowed { get; set; }
    
    public int FolloweeFollowerCount { get; set; }
}