using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappers;

public static class PostLikeMapper
{
    public static Expression<Func<PostLike, UserPreviewDto>> ProjectToUserPreviewDto => 
        like => new UserPreviewDto
        {
            Id = like.User.Id,
            Username = like.User.Status != Domain.Enums.UserStatus.Deactivated
                ? like.User.UsernameNormalized 
                : "deleted",
            ThumbnailProfilePicStorageKey = like.User.CurrentProfilePic != null 
                ? like.User.CurrentProfilePic.ThumbnailStorageKey 
                : null,
        };
}