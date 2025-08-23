using MediatR;
using SocialMedia.Application.DTOs;
using SocialMedia.Shared;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.Register;

public sealed record RegisterUserCommand(
    string Username, 
    string Email, 
    DateTime BirthDate,
    string RawPassword) : IRequest<Result<RegisterResponse>>;