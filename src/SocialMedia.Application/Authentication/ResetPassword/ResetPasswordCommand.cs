using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : IRequest<Result<MessageResponse>>;