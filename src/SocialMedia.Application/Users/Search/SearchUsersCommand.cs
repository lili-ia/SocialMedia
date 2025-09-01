using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Users.Search;

public sealed record SearchUsersCommand(
    Guid? ForUserId,
    string Username,
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;