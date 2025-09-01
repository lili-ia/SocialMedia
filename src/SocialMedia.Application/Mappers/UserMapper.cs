using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappers;

public static class UserMapper
{
    public static UpdateUserDto ToUpdateUserDto(this User user) => new()
    {
        Username = user.Username,
        BirthDate = user.BirthDate,
        Email = user.Email,
        ProfilePicUrl = user.ProfilePic?.Url,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt
    };

    public static Expression<Func<User, UserPrivateDto>> ToUserPrivateDto => 
        user => new UserPrivateDto 
        {
            Username = user.Username,
            ProfilePicUrl = user.ProfilePic.Url,
            Bio = user.Bio,
            PostsCount = user.Posts.Count(p => p.User.Status == UserStatus.Active && p.IsActive),
            FollowersCount = user.Followers.Count(f => f.Follower.Status == UserStatus.Active),
            FolloweesCount = user.Followees.Count(f => f.Followee.Status == UserStatus.Active),
            BirthDate = user.BirthDate,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    
    public static Expression<Func<User, UserPublicDto>> ToUserPublicDto => 
        user => new UserPublicDto 
        {
            Username = user.Username,
            ProfilePicUrl = user.ProfilePic.Url,
            Bio = user.Bio,
            PostsCount = user.Posts.Count(p => p.User.Status == UserStatus.Active && p.IsActive),
            FollowersCount = user.Followers.Count(f => f.Follower.Status == UserStatus.Active),
            FolloweesCount = user.Followees.Count(f => f.Followee.Status == UserStatus.Active)
        };
    
    public static Expression<Func<User, UserPreviewDto>> ToUserPreviewDto => 
        user => new UserPreviewDto
        {
            Id = user.Id,
            Username = user.Username,
            ProfilePicUrl = user.ProfilePic.Url
        };
}
    
    