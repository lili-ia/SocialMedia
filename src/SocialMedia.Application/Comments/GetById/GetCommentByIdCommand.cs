using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Comment;

namespace SocialMedia.Application.Comments.GetById;

public sealed record GetCommentByIdCommand(
    Guid CommentId, 
    Guid TargetUserId) : IRequest<Result<CommentWithAuthorDto>>;