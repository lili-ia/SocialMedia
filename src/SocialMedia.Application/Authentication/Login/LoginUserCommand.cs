using MediatR;
using SocialMedia.Application.DTOs;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.Login;

public sealed record LoginUserCommand(
    string Email, 
    string Password,
    string IpAddress,
    string DeviceInfo) : IRequest<Result<AuthResponse>>;