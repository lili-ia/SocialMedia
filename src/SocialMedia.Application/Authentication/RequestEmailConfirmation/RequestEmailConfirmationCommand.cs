using MediatR;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestEmailConfirmation;

public sealed record RequestEmailConfirmationCommand(
    string Email
) : IRequest<Result>;