using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Comments.Delete;

public sealed record DeleteCommentByIdCommand(
    Guid CommentId, 
    Guid UserId) : IRequest<Result>;