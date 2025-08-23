using MediatR;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : IRequest<Result>;