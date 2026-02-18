using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Posts;

namespace SocialMedia.Application.Users.Update;

public sealed record UpdateUserCommand(
    Guid UserId,
    DateOnly? BirthDate,
    FileData? ProfilePic,
    string? Bio) : IRequest<Result<UpdateUserDto>>;