using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Likes.DeleteLike;

public sealed record DeletePostLikeCommand(
    Guid LikerId, 
    Guid PostId) : IRequest<Result>;