using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ISendMessageUseCase
{
    Task<Message> ExecuteAsync(int chatId, string content, int senderId, CancellationToken ct);
}