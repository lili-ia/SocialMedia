using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Messages.Send;

public class SendMessageCommandHandler(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IFileStorageService storageService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<SendMessageCommandHandler> logger)
    : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var chat = await chatRepository.GetByIdWithParticipantsAsync(request.ChatId, ct);

        if (chat is null || !chat.IsParticipant(request.SenderId))
        {
            logger.LogWarning("Chat {ChatId} not found or user {SenderId} is not a participant.", request.ChatId, request.SenderId);

            return Result<MessageDto>.Failure("Chat not found.", ErrorType.NotFound);
        }
        
        var message = Message.Create(request.ChatId, request.SenderId, request.Text!, request.ParentMessageId);
        
        if (request.Attachments is { Count: > 0 })
        {
            try
            {
                var postFiles = await Task.WhenAll(request.Attachments.Select(async f =>
                {
                    byte[] bytes;
                    
                    await using (var ms = new MemoryStream())
                    {
                        await f.Content.CopyToAsync(ms, ct);
                        bytes = ms.ToArray();
                    }
                    
                    var storageKey = await storageService.UploadFileAsync(
                        f.FileName, 
                        new MemoryStream(bytes), 
                        MediaFolder.PostFiles, 
                        ct);
                    
                    return MessageAttachment.Create(
                        request.SenderId,
                        message.Id,
                        f.FileName,
                        ContentType.Image, 
                        storageKey, 
                        bytes.Length);
                }));
                
                message.AddAttachments(postFiles);
            }
            catch (FileStorageException ex) // todo implement a background job that will cleanup orphaned files
            {
                logger.LogError(ex, "S3 Upload failed for user {UserId}", request.SenderId);
                
                return Result<MessageDto>.InternalError("An error occurred while uploading images.");
            }
        }

        await messageRepository.AddAsync(message, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Message {MessageId} sent by {SenderId} in chat {ChatId}.",
            message.Id, request.SenderId, request.ChatId);

        var sender = await userRepository.GetByIdAsync(request.SenderId, ct);

        if (sender is null)
        {
            return Result<MessageDto>.Failure("User not found.", ErrorType.NotFound);
        }
        
        var dto = message.ToDto(sender);

        foreach (var attachment in dto.Attachments)
        {
            attachment.Url = storageService.GetPresignedUrl(attachment.StorageKey);
        }

        return Result<MessageDto>.Success(dto);
    }
}