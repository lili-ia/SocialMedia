using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ISendMessageUseCase
{
    Task<Message> ExecuteAsync(Guid chatId, string content, Guid senderId, CancellationToken ct);
}