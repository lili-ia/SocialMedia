using FluentValidation;

namespace SocialMedia.Application.Chats.AddParticipant;

public class AddParticipantCommandValidator : AbstractValidator<AddParticipantCommand>
{
    public AddParticipantCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.NewUserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required.");

        RuleFor(x => x)
            .Must(x => x.RequesterId != x.NewUserId)
            .WithMessage("You cannot add yourself to the chat.");
    }
}