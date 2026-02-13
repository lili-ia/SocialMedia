using Domain.Entities;
using FluentValidation;
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
using ContentType = Domain.Enums.ContentType;

namespace SocialMedia.Application.Posts.Create;

public class CreatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreatePostCommand> validator,
    ILogger<CreatePostCommandHandler> logger,
    IFileStorageService fileStorage,
    IFileRepository fileRepository)
    : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<PostDto>();
        }

        var post = new Post
        {
            Text = request.Text,
            IsHidden = false,
            UserId = request.UserId
        };
        
        await postRepository.AddAsync(post, ct);

        List<string> presignedUrls = [];

        if (request.Files is { Count: > 0 })
        {
            try
            {
                var postFiles = await Task.WhenAll(request.Files.Select(async f =>
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

                presignedUrls = postFiles
                    .Select(f => fileStorage.GetPresignedUrl(f.OriginalStorageKey, 60))
                    .ToList();
            }
            catch (FileStorageException ex) // todo implement a background job that will cleanup orphaned files
            {
                logger.LogError(ex, "S3 Upload failed for user {UserId}", request.UserId);
                
                return Result<PostDto>.InternalError("An error occurred while uploading images.");
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        
        logger.LogInformation("Post {PostId} created by user {UserId}.", post.Id, request.UserId);

        return Result<PostDto>.Success(post.ToDto(null, presignedUrls));
    }
}