using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Chat;

namespace SocialMedia.Application.Chats.Create;

public record CreateChatCommand : IRequest<Result<ChatDto>>
{
    public Guid RequesterId { get; init; }
    
    public bool IsGroup { get; init; }
    
    public string? GroupName { get; init; } // required if IsGroup
    
    public List<Guid> ParticipantIds { get; init; } = [];
}