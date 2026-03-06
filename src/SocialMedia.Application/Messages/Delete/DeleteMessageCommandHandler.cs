using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Messages.Delete;

public class DeleteMessageCommandHandler(
    IMessageRepository messageRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteMessageCommandHandler> logger)
    : IRequestHandler<DeleteMessageCommand, Result>
{
    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken ct)
    {
        var message = await messageRepository.GetByIdAsync(request.MessageId, ct);

        if (message is null)
        {
            return Result.Failure("Message not found.", ErrorType.NotFound);
        }

        message.Delete(request.RequesterId);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Message {MessageId} deleted by {RequesterId}.", request.MessageId, request.RequesterId);

        return Result.Success();
    }
}