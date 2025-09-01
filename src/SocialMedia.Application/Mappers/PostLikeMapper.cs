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
                ? like.User.Username 
                : "deleted",
            ProfilePicUrl = like.User.ProfilePic.Url
        };
}