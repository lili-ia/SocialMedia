using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace Infrastructure.Hubs;

[Authorize]
public class ChatHub(
    IUserContext userContext,
    IChatRepository chatRepository,
    PresenceTracker presenceTracker) : Hub
{
    public const string SendMessageEvent = "SendMessage";
    public const string TypingEvent = "Typing";
    public const string StopTypingEvent = "StopTyping";
    public const string JoinChatEvent = "JoinChat";
    public const string LeaveChatEvent = "LeaveChat";

    public const string ReceiveMessageEvent = "ReceiveMessage";
    public const string UserTypingEvent = "UserTyping";
    public const string UserStoppedTypingEvent = "UserStoppedTyping";
    public const string UserOnlineEvent = "UserOnline";
    public const string UserOfflineEvent = "UserOffline";
    public const string ReceiveNotificationEvent = "ReceiveNotification";

    public override async Task OnConnectedAsync()
    {
        var userId = userContext.UserId;
        await presenceTracker.UserConnectedAsync(userId, Context.ConnectionId);

        var chats = await chatRepository.GetChatsForUserAsync(
            userId, c => c.Id, Context.ConnectionAborted);

        foreach (var chatId in chats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));
        }

        await Clients.Others.SendAsync(UserOnlineEvent, userId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = userContext.UserId;
        await presenceTracker.UserDisconnectedAsync(userId, Context.ConnectionId);

        var isStillOnline = await presenceTracker.IsOnlineAsync(userId);
        
        if (!isStillOnline)
        {
            await Clients.Others.SendAsync(UserOfflineEvent, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(Guid chatId)
    {
        var isParticipant = await chatRepository.IsParticipantAsync(chatId, userContext.UserId);

        if (!isParticipant)
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }

    public async Task LeaveChat(Guid chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));
    }

    public async Task Typing(Guid chatId)
    {
        await Clients.OthersInGroup(ChatGroup(chatId))
            .SendAsync(UserTypingEvent, userContext.UserId, chatId);
    }

    public async Task StopTyping(Guid chatId)
    {
        await Clients.OthersInGroup(ChatGroup(chatId))
            .SendAsync(UserStoppedTypingEvent, userContext.UserId, chatId);
    }

    public static string ChatGroup(Guid chatId) => $"chat:{chatId}";
    
    public static string UserGroup(Guid userId) => $"user:{userId}";
}