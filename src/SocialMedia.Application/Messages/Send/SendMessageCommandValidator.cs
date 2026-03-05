using FluentValidation;

namespace SocialMedia.Application.Messages.Send;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    private const int MaxTextLength = 2000;
    private const int MaxAttachments = 10;
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Chat ID is required.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Text) || x.Attachments.Count > 0)
            .WithMessage("Message must contain text or at least one attachment.");

        When(x => x.Text is not null, () =>
        {
            RuleFor(x => x.Text)
                .MaximumLength(MaxTextLength)
                .WithMessage($"Message text must not exceed {MaxTextLength} characters.");
        });

        RuleFor(x => x.Attachments)
            .Must(a => a.Count <= MaxAttachments)
            .WithMessage($"Cannot send more than {MaxAttachments} attachments per message.");

        RuleForEach(x => x.Attachments).ChildRules(attachment =>
        {
            attachment.RuleFor(a => a.FileName)
                .NotEmpty().WithMessage("Attachment file name is required.")
                .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

            attachment.RuleFor(a => a.Content.Length)
                .GreaterThan(0).WithMessage("Attachment size must be greater than 0.")
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"Each attachment must not exceed {MaxFileSizeBytes / 1024 / 1024} MB.");
        });
    }
}