using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserDto>>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IFileStorageService _fileStorage;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger, 
        IUserRepository userRepository, 
        IValidator<UpdateUserCommand> validator, 
        IFileStorageService fileStorage, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userRepository = userRepository;
        _validator = validator;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateUserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateUserCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateUserCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<UpdateUserDto>();
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UpdateUserDto>.Failure("User not found.", ErrorType.NotFound);
        }

        string? profilePicUrl = null;
        
        if (request.ProfilePic is not null)
        {
            try
            {
                profilePicUrl = await _fileStorage
                    .UploadFileAsync(request.ProfilePic.FileName, request.ProfilePic.Content, cancellationToken);

                if (user.ProfilePic is null)
                {
                    user.ProfilePic = new ProfilePic
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        FileName = request.ProfilePic.FileName,
                        ContentType = ContentType.Image,
                        Url = profilePicUrl
                    };
                }
                else
                {
                    await _fileStorage.DeleteFileAsync(user.ProfilePic.Url, cancellationToken);

                    user.ProfilePic.FileName = request.ProfilePic.FileName;
                    user.ProfilePic.ContentType = ContentType.Image;
                    user.ProfilePic.Url = profilePicUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while uploading the profile picture.");
                
                return Result<UpdateUserDto>.Failure("An internal error occured.", ErrorType.ServerError);
            }
        }

        user.Bio = request.Bio ?? user.Bio;
        user.BirthDate = request.BirthDate ?? user.BirthDate;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} profile successfully updated.", request.UserId);

            var userDto = user.ToUpdateUserDto();
            
            return Result<UpdateUserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while updating user {UserId} profile.", request.UserId);

            if (profilePicUrl is not null)
            {
                await DeleteUploadedProfilePicAsync(profilePicUrl, cancellationToken);
            }

            return Result<UpdateUserDto>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
    private async Task DeleteUploadedProfilePicAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            await _fileStorage.DeleteFileAsync(url, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete orphaned file {FileUrl}.", url);
        }
    }
}