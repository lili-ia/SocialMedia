using MediatR;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Token
) : IRequest<Result>;