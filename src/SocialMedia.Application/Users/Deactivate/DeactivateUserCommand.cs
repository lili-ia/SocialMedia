using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Users.Deactivate;

public sealed record DeactivateUserCommand(Guid UserId) : IRequest<Result>;