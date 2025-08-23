using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterResponse>>
{
    private readonly IValidator<RegisterUserCommand> _validator;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterUserCommandHandler(
        IValidator<RegisterUserCommand> validator, 
        ILogger<RegisterUserCommandHandler> logger, 
        IUserRepository userRepository, 
        IPasswordService passwordService, 
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<RegisterResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to register a user with email {Email}.", request.Email);

        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<RegisterResponse>();
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        var normalizedUsername = request.Username.Trim().ToLower();
        
        var existingUser =
            await _userRepository.GetByEmailOrUsernameAsync(normalizedEmail, normalizedUsername, cancellationToken);

        if (existingUser is not null)
        {
            return Result<RegisterResponse>.Failure(existingUser.Email == normalizedEmail
                ? "User with this email already exists." : "User with this username already exists.", ErrorType.Conflict);
        }
        
        var passwordHash = _passwordService.HashPassword(request.RawPassword);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = normalizedUsername,
            BirthDate = request.BirthDate,
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Status = UserStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UserRole = UserRole.User,
        };

        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("User with email {Email} registered successfully. UserId: {UserId}", 
                normalizedEmail, user.Id);

            var response = new RegisterResponse
            {
                Id = user.Id,
                Username = normalizedUsername,
                Email = normalizedEmail,
                Status = UserStatus.Pending,
                UserRole = UserRole.User
            };
            
            return Result<RegisterResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while registering user with email {Email}.", normalizedEmail);
            
            return Result<RegisterResponse>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}