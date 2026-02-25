using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappers;

public static class CommentMapper
{
    public static CommentWithAuthorDto ToDto(this Comment comment, User user) => new()
    {
        UserPreview = new UserPreviewDto
        {
            Id = user.Id,
            Username = user.UsernameNormalized,
            ThumbnailProfilePicStorageKey = user.CurrentProfilePic.ThumbnailStorageKey
        },
        Text = comment.Text,
        PostId = comment.PostId,
        CreatedAt = comment.CreatedAt
    };
    
    public static CommentWithAuthorDto ToDto(this Comment comment) => new()
    {
        UserPreview = null,
        Text = null,
        PostId = default,
        CreatedAt = default
    };
    
    public static Expression<Func<Comment, CommentWithAuthorDto>> ProjectToCommentWithAuthorDto =>
        comment => new CommentWithAuthorDto
        {
            UserPreview = new UserPreviewDto
            {
                Id = comment.UserId,
                Username = comment.User.UsernameNormalized,
                ThumbnailProfilePicStorageKey = comment.User.CurrentProfilePic.ThumbnailStorageKey
            },
            Text = comment.Text,
            PostId = comment.PostId,
            CreatedAt = comment.CreatedAt
        };
}