using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappers;

public static class FollowMapper
{
    public static Expression<Func<Follow, UserPreviewDto>> ToFolloweeUserPreviewDto => 
        follow => new UserPreviewDto
        {
            Id = follow.Followee.Id,
            Username = follow.Followee.Username,
            ProfilePicUrl = follow.Followee.ProfilePic.Url
        };
    
    public static Expression<Func<Follow, UserPreviewDto>> ToFollowerUserPreviewDto => 
        follow => new UserPreviewDto
        {
            Id = follow.Follower.Id,
            Username = follow.Follower.Username,
            ProfilePicUrl = follow.Follower.ProfilePic.Url
        };
}