using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Likes.GetPostLikers;

public sealed record GetPostLikersCommand(
    Guid PostId, 
    Guid TargetUserId,
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;