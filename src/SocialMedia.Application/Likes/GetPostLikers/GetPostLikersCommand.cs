using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Likes.GetPostLikers;

public sealed record GetPostLikersCommand(
    Guid PostId, 
    Guid TargetUserId) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;