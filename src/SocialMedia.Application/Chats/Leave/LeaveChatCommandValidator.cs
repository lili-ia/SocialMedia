using FluentValidation;

namespace SocialMedia.Application.Chats.Leave;

public class LeaveChatCommandValidator : AbstractValidator<LeaveChatCommand>
{
    public LeaveChatCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}