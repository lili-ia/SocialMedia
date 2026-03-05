using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Mappers;

public static class PostMapper
{
    public static Expression<Func<Post, PostDto>> ProjectToDto => 
        post => new PostDto
        {
            PostId = post.Id,
            Text = post.Text,
            UserId = post.UserId,
            Username = post.User.UsernameNormalized,
            IsHidden = post.IsHidden,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            CommentCount = post.CommentCount,
            LikeCount = post.LikeCount,
            ViewCount = post.ViewCount,
            FileStorageKeys = post.PostFiles
                .Select(f => f.StorageKey)
                .ToList()
        };

    public static PostDto ToDto(this Post post, string? username = null, List<string>? fileUrls = null) => new PostDto
    {
        PostId = post.Id,
        Text = post.Text,
        UserId = post.UserId,
        Username = username,
        IsHidden = post.IsHidden,
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt,
        CommentCount = post.CommentCount,
        LikeCount = post.LikeCount,
        ViewCount = 0,
        FileUrls = fileUrls
    };
}