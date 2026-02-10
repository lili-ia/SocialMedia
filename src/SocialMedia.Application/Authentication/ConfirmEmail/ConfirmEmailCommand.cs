using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Authentication.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Token
) : IRequest<Result<MessageResponse>>;