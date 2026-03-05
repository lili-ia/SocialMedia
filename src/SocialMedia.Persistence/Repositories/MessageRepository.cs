using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class MessageRepository(SocialMediaDbContext db) : IMessageRepository
{
    public async Task<Message?> GetByIdAsync(Guid messageId, CancellationToken ct = default)
    {
        return await db.Messages.FirstOrDefaultAsync(m => m.Id == messageId, ct);
    }

    public async Task<IReadOnlyList<TResult>> GetMessagesForChatAsync<TResult>(
        Guid chatId, 
        Expression<Func<Message, TResult>> selector, 
        int skip, 
        int take,
        CancellationToken ct = default)
    {
        return await db.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Message message, CancellationToken ct = default)
    {
        await db.Messages.AddAsync(message, ct);
    }
}