using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Comment;

namespace SocialMedia.Application.Comments.Create;

public sealed record CreateCommentCommand(
    string Text, 
    Guid PostId, 
    Guid UserId,
    Guid? ParentCommentId) : IRequest<Result<CommentWithAuthorDto>>;