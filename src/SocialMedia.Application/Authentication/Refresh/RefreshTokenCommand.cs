using MediatR;
using SocialMedia.Application.DTOs;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.Refresh;

public sealed record  RefreshTokenCommand(
    string RefreshToken, 
    string IpAddress, 
    string DeviceInfo) : IRequest<Result<AuthResponse>>;