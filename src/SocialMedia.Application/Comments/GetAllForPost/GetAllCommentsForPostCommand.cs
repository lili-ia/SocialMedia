using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Comment;

namespace SocialMedia.Application.Comments.GetAllForPost;

public sealed record GetAllCommentsForPostCommand(
    Guid PostId, 
    Guid TargetUserId,
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<CommentWithAuthorDto>>>;