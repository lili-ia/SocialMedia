using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Posts.Create;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<Guid>>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePostCommand> _validator;
    private readonly ILogger<CreatePostCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorage;

    public CreatePostCommandHandler(
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork, 
        IValidator<CreatePostCommand> validator,
        ILogger<CreatePostCommandHandler> logger,
        IUserRepository userRepository, 
        IFileStorageService fileStorage)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _fileStorage = fileStorage;
    }
    
    public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreatePostCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePostCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<Guid>();
        }

        var isActive = await _userRepository.IsActive(request.UserId, UserRole.User, cancellationToken);

        if (!isActive)
        {
            _logger.LogWarning("User {UserId} can not upload posts unless they have Active user status.", request.UserId);
            
            return Result<Guid>.Failure("Access denied.", ErrorType.Forbidden);
        }
        
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            UserId = request.UserId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var uploadedFiles = new List<string>();
        
        if (request.Files is { Count: > 0 })
        {
            try
            {
                var files = await Task.WhenAll(request.Files.Select(async f =>
                {
                    var url = await _fileStorage.UploadFileAsync(f.FileName, f.Content, cancellationToken);
                    uploadedFiles.Add(url);

                    return new PostFile
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        FileName = f.FileName,
                        ContentType = ContentType.Image,
                        Url = url
                    };
                }));

                post.PostFiles = files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to upload a file to the file storage a post.");
                
                return Result<Guid>.Failure("An internal error occured.", ErrorType.ServerError);
            }
        }

        try
        {
            await _postRepository.AddAsync(post, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Post {PostId} successfully created by user {UserId}.", post.Id, request.UserId);

            return Result<Guid>.Success(post.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating a post by user {UserId}.", request.UserId);
            
            await DeleteUploadedFilesAsync(uploadedFiles, cancellationToken);
            
            return Result<Guid>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
    
    private async Task DeleteUploadedFilesAsync(IEnumerable<string> urls, CancellationToken cancellationToken)
    {
        foreach (var url in urls)
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
}