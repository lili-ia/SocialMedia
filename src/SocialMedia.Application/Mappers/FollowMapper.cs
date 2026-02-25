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
            Username = follow.Followee.UsernameNormalized,
            ThumbnailProfilePicUrl = follow.Followee.CurrentProfilePic.ThumbnailStorageKey
        };
    
    public static Expression<Func<Follow, UserPreviewDto>> ToFollowerUserPreviewDto => 
        follow => new UserPreviewDto
        {
            Id = follow.Follower.Id,
            Username = follow.Follower.UsernameNormalized,
            ThumbnailProfilePicUrl = follow.Follower.CurrentProfilePic.ThumbnailStorageKey
        };
}