using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Comment;

namespace SocialMedia.Application.Mappers;

public static class CommentMapper
{
    public static CommentDto ToDto(this Comment comment) => new()
    {
        Text = comment.Text,
        UserId = comment.UserId,
        PostId = comment.PostId,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt
    };
    
    public static CommentDto ToDto(this Comment comment, string username) => new()
    {
        Text = comment.Text,
        UserId = comment.UserId,
        Username = username,
        PostId = comment.PostId,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt
    };
    
    public static Expression<Func<Comment, CommentDto>> ProjectToDto => 
        comment => new CommentDto
        {
            Text = comment.Text,
            UserId = comment.UserId,
            Username = comment.User.Username,
            PostId = comment.PostId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
}