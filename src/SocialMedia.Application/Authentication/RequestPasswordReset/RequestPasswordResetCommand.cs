using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestPasswordReset;

public sealed record RequestPasswordResetCommand(string Email) : IRequest<Result>;