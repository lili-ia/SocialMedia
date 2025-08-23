using MediatR;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestPasswordReset;

public sealed record RequestPasswordResetCommand(string Email) : IRequest<Result>;