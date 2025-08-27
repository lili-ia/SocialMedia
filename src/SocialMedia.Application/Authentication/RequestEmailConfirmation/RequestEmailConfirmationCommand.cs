using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestEmailConfirmation;

public sealed record RequestEmailConfirmationCommand(
    string Email
) : IRequest<Result>;