using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class ChatRepository(SocialMediaDbContext db) : IChatRepository
{
    public async Task<Chat?> GetByIdWithParticipantsAsync(Guid chatId, CancellationToken ct = default)
    {
        return await db.Chats
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == chatId, ct);
    }

    public async Task<Chat?> GetDirectChatBetweenAsync(Guid userAId, Guid userBId, CancellationToken ct = default)
    {
        return await db.Chats
            .Where(c =>
                c.Type == ChatType.Direct &&
                c.Participants.Any(p => p.UserId == userAId && p.IsActive) &&
                c.Participants.Any(p => p.UserId == userBId && p.IsActive))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> GetChatsForUserAsync<TResult>(
        Guid userId, 
        Expression<Func<Chat, TResult>> selector, 
        CancellationToken ct = default)
    {
        return await db.Chats
            .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive))
            .OrderByDescending(c => c.Messages
                .Where(m => m.Status == MessageStatus.Sent)
                .Max(m => m.CreatedAt))
            .Select(selector)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Chat chat, CancellationToken ct = default)
    {
        await db.Chats.AddAsync(chat, ct);
    }

    public async Task<bool> IsParticipantAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await db.Chats.AnyAsync(c =>
            c.Id == chatId &&
            c.Participants.Any(p => p.UserId == userId && p.IsActive), ct);
    }
}