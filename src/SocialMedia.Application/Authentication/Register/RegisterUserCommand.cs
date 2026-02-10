using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Register;

public sealed record RegisterUserCommand(
    string Username, 
    string Email, 
    DateTime BirthDate,
    string RawPassword) : IRequest<Result<MessageResponse>>;