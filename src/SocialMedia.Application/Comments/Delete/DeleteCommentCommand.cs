using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Comments.Delete;

public sealed record DeleteCommentCommand(
    Guid CommentId, 
    Guid UserId) : IRequest<Result<MessageResponse>>;