using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Update;

public class UpdateUserCommandHandler(
    ILogger<UpdateUserCommandHandler> logger,
    IUserRepository userRepository,
    IFileStorageService fileStorage,
    IUnitOfWork unitOfWork,
    IFileRepository fileRepository)
    : IRequestHandler<UpdateUserCommand, Result<UpdateUserDto>>
{
    public async Task<Result<UpdateUserDto>> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct, tracking: true);
        
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UpdateUserDto>.Failure("User not found.", ErrorType.NotFound);
        }
        
        string? newOriginalKey = null;
        
        if (request.ProfilePic is not null)
        {
            try
            {
                using var ms = new MemoryStream();
                await request.ProfilePic.Content.CopyToAsync(ms, ct);
                var bytes = ms.ToArray();

                var originalKey = await fileStorage.UploadFileAsync(
                    request.ProfilePic.FileName, 
                    new MemoryStream(bytes), 
                    MediaFolder.ProfilePics, ct);

                using var image = Image.Load(bytes);
                var originalWidth = image.Width;
                var originalHeight = image.Height;

                image.Mutate(x => 
                    x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(200, 200)
                    }));
            
                using var thumbMs = new MemoryStream();
                await image.SaveAsJpegAsync(thumbMs, ct);
                var thumbnailSize = thumbMs.Length;
                thumbMs.Position = 0;
                
                var thumbKey = await fileStorage.UploadFileAsync(
                    $"thumb_{request.ProfilePic.FileName}", 
                    thumbMs, 
                    MediaFolder.ProfilePics, ct);

                var newProfilePic = ProfilePic.Create(
                    user.Id, 
                    request.ProfilePic.FileName,
                    ContentType.Image, 
                    originalKey, 
                    bytes.Length,
                    thumbKey,
                    thumbnailSize,
                    originalWidth,
                    originalHeight);

                newOriginalKey = originalKey;
                
                await fileRepository.AddAsync(newProfilePic, ct);

                user.UpdateProfilePicture(newProfilePic.Id);
            }
            catch (FileStorageException ex)
            {
                logger.LogError(ex, "S3 Upload failed for user {UserId}", request.UserId);
                
                return Result<UpdateUserDto>.InternalError("An error occurred while uploading profile pic.");
            }
        }
        
        user.UpdateProfile(request.Bio, request.BirthDate);
        await unitOfWork.SaveChangesAsync(ct);

        var dto = user.ToUpdateUserDto();
        
        if (newOriginalKey is not null)
        {
            dto.ProfilePicUrl = fileStorage.GetPresignedUrl(newOriginalKey);
        }
        
        return Result<UpdateUserDto>.Success(dto);
    }
}