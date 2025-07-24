using Microsoft.AspNetCore.SignalR;
using SocialMedia.Application.Contracts;

namespace SocialMedia;

public class ChatHub : Hub
{
    private readonly ISendMessageUseCase _sendMessageUseCase;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ISendMessageUseCase sendMessageUseCase, ILogger<ChatHub> logger)
    {
        _sendMessageUseCase = sendMessageUseCase;
        _logger = logger;
    }

    public async Task SendMessage(Guid chatId, string content)
    {
        var cancellationToken = Context.ConnectionAborted;
        
        var userStringId = Context.UserIdentifier;

        _logger.LogInformation("User {UserIdentifier} is attempting to send a message to chat {ChatId}.", userStringId, chatId);

        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogWarning("User {UserIdentifier} tried to send an empty message to chat {ChatId}.", userStringId, chatId);
            await Clients.Caller.SendAsync("Error", "Message content cannot be empty.");
            
            return;
        }

        if (userStringId != null && Guid.TryParse(userStringId, out Guid userGuidId))
        {
            try
            {
                var message = await _sendMessageUseCase.ExecuteAsync(chatId, content, userGuidId, cancellationToken);
                _logger.LogInformation(
                    "User {UserIdentifier} successfully sent a message to chat {ChatId}.", 
                    userStringId, chatId);
                await Clients.All.SendAsync("ReceiveMessage",userGuidId,  message.Content, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error while sending message in chat {ChatId} by user {UserId}.", 
                    chatId, userGuidId);
                await Clients.Caller
                    .SendAsync("Error", "Something went wrong while sending the message.");
            }
        }
        else
        {
            _logger.LogWarning(
                "Unauthorized access attempt with invalid user ID: {UserIdentifier}.", 
                userStringId);
            await Clients.Caller
                .SendAsync("Error", "User is not authenticated or ID is invalid.");
        }
    }
}