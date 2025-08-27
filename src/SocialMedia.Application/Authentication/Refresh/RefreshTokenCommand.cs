using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Refresh;

public sealed record  RefreshTokenCommand(
    string RefreshToken, 
    string IpAddress, 
    string DeviceInfo) : IRequest<Result<AuthResponse>>;