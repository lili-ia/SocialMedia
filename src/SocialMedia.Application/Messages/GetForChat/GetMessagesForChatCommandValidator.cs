using FluentValidation;

namespace SocialMedia.Application.Messages.GetForChat;

public class GetMessagesForChatCommandValidator : AbstractValidator<GetMessagesForChatCommand>
{
    private const int MaxPageSize = 100;

    public GetMessagesForChatCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"Page size must be between 1 and {MaxPageSize}.");
    }
}