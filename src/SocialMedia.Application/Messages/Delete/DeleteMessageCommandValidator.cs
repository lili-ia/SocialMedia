using FluentValidation;

namespace SocialMedia.Application.Messages.Delete;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("Message ID is required.");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required.");
    }
}