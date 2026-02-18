using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappers;

public static class UserMapper
{
    public static UpdateUserDto ToUpdateUserDto(this User user) => new()
    {
        Username = user.UsernameNormalized,
        BirthDate = user.BirthDate,
        Email = user.EmailNormalized,
        ProfilePicUrl = null,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    public static Expression<Func<User, UserPrivateDto>> ToUserPrivateDto => 
        user => new UserPrivateDto
        {
            Username = user.UsernameNormalized,
            ProfilePicStorageKey = user.CurrentProfilePic != null 
                ? user.CurrentProfilePic.OriginalStorageKey 
                : null,
            Bio = user.Bio,
            PostsCount = user.Posts.Count,
            FollowersCount = user.Followers.Count,
            FolloweesCount = user.Followees.Count,
            BirthDate =  user.BirthDate,
            Email = user.EmailNormalized,
            CreatedAt = user.CreatedAt
        };
    
    public static Expression<Func<User, UserPublicDto>> ToUserPublicDto => 
        user => new UserPublicDto
        {
            Username = user.UsernameNormalized,
            ProfilePicStorageKey = user.CurrentProfilePic != null 
                ? user.CurrentProfilePic.OriginalStorageKey 
                : null,
            Bio = user.Bio,
            PostsCount = user.Posts.Count,
            FollowersCount = user.Followers.Count,
            FolloweesCount = user.Followees.Count,
        };
    
    public static Expression<Func<User, UserPreviewDto>> ToUserPreviewDto => 
        user => new UserPreviewDto
        {
            Id = user.Id,
            Username = user.UsernameNormalized,
            ThumbnailProfilePicStorageKey = user.CurrentProfilePic != null 
                ? user.CurrentProfilePic.ThumbnailStorageKey 
                : null,
        };
}
    
    