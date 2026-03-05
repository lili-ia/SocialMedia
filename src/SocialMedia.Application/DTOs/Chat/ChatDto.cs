using Domain.Enums;

namespace SocialMedia.Application.DTOs.Chat;

public class ChatDto
{
    public Guid Id { get; set; }
    
    public ChatType Type { get; set; }
    
    public string? Name { get; set; }
    
    public List<ChatParticipantDto> Participants { get; set; } = [];
    
    public MessageDto? LastMessage { get; set; }
    
    public int UnreadCount { get; set; }
}