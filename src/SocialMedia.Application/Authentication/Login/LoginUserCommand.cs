using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Login;

public sealed record LoginUserCommand(
    string Email, 
    string Password,
    string IpAddress,
    string DeviceInfo) : IRequest<Result<AuthResponse>>;