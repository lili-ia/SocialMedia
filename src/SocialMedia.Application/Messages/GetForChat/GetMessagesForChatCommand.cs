using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Chat;

namespace SocialMedia.Application.Messages.GetForChat;

public record GetMessagesForChatCommand : IRequest<Result<IReadOnlyList<MessageDto>>>, ICacheable
{
    public Guid RequesterId { get; init; }
    
    public Guid ChatId { get; init; }
    
    public int Page { get; init; }
    
    public int PageSize { get; init; }
    
    public string CacheKey => $"messages:chat:{ChatId}:page:{Page}:size:{PageSize}";
    
    public TimeSpan Ttl => TimeSpan.FromMinutes(2);
}