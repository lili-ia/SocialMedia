using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.Posts;

namespace SocialMedia.Application.Messages.Send;

public record SendMessageCommand(
    Guid SenderId, 
    Guid ChatId, 
    string? Text, 
    Guid? ParentMessageId, 
    List<FileData>? Attachments) 
    : IRequest<Result<MessageDto>>;