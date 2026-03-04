using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.Update;

public class UpdatePostCommandHandler(
    ILogger<UpdatePostCommandHandler> logger,
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorage,
    IFileRepository fileRepository)
    : IRequestHandler<UpdatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdWithFilesAsync(request.PostId, ct);

        if (post is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != request.UserId)
        {
            logger.LogWarning("User {UserId} unauthorized for post {PostId}.", request.UserId, request.PostId);
            
            return Result<PostDto>.Failure("You do not own this post.", ErrorType.Forbidden);
        }
        
        var keptKeys = request.KeptStorageKeys ?? [];
        
        var filesToRemove = post.PostFiles
            .Where(f => !keptKeys.Contains(f.OriginalStorageKey))
            .ToList();

        foreach (var file in filesToRemove)
        {
            await fileRepository.RemoveAsync(file, ct);
            await fileStorage.DeleteFileAsync(file.OriginalStorageKey, ct);
        }
        
        if (request.NewFiles is { Count: > 0 })
        {
            try
            {
                var postFiles = await Task.WhenAll(request.NewFiles.Select(async f =>
                {
                    byte[] bytes;
                    
                    await using (var ms = new MemoryStream())
                    {
                        await f.Content.CopyToAsync(ms, ct);
                        bytes = ms.ToArray();
                    }
                    
                    var storageKey = await fileStorage.UploadFileAsync(f.FileName, new MemoryStream(bytes), MediaFolder.PostFiles, ct);
                    
                    var info = await Image.IdentifyAsync(new MemoryStream(bytes), ct);
                    
                    return new PostFile 
                    {
                        UserId = request.UserId,
                        OriginalFileName = f.FileName,
                        ContentType = ContentType.Image,
                        OriginalStorageKey = storageKey,
                        OriginalFileSize = bytes.Length,
                        PostId = post.Id,
                        Width = info.Width,
                        Height = info.Height
                    };
                }));
                
                await fileRepository.AddRangeAsync(postFiles, ct);
            }
            catch (FileStorageException ex) 
            {
                logger.LogError(ex, "S3 Upload failed for user {UserId}", request.UserId);
                
                return Result<PostDto>.InternalError("An error occurred while uploading images.");
            }
        }
        
        post.UpdateText(request.Text);

        await unitOfWork.SaveChangesAsync(ct);
        
        logger.LogInformation("Post {PostId} successfully updated.", post.Id);

        var dto = await postRepository.GetDetailsAsync(request.PostId, PostMapper.ProjectToDto, ct);

        if (dto is null)
        {
            return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        dto.FileUrls = dto.FileStorageKeys?.Select(key => fileStorage.GetPresignedUrl(key, 60)).ToList();

        return Result<PostDto>.Success(dto);
    }
}