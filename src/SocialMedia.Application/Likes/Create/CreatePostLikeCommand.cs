using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Like;

namespace SocialMedia.Application.Likes.Create;

public sealed record CreatePostLikeCommand(
    Guid LikerId,
    Guid PostId) : IRequest<Result<PostLikeResponse>>;