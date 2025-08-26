using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Mappers;

public static class PostMapper
{
    public static Expression<Func<Post, PostDto>> ToDto => 
        post => new PostDto
        {
            PostId = post.Id,
            Text = post.Text,
            UserId = post.UserId,
            Username = post.User.Username,
            IsActive = post.IsActive,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            CommentsCount = post.Comments.Count,
            LikesCount = post.PostLikes.Count
        };
}