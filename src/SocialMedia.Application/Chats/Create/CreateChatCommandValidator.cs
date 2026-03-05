using FluentValidation;

namespace SocialMedia.Application.Chats.Create;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    private const int MaxParticipants = 50;

    public CreateChatCommandValidator()
    {
        When(x => x.IsGroup, () =>
        {
            RuleFor(x => x.GroupName)
                .NotEmpty().WithMessage("Group name is required.")
                .MaximumLength(100).WithMessage("Group name must not exceed 100 characters.");

            RuleFor(x => x.ParticipantIds)
                .NotEmpty().WithMessage("Group chat requires at least 2 participants.")
                .Must(ids => ids.Count >= 2)
                .WithMessage("Group chat requires at least 2 other participants.")
                .Must(ids => ids.Count <= MaxParticipants)
                .WithMessage($"Group chat cannot exceed {MaxParticipants} participants.")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Participant list contains duplicates.");
        });

        When(x => !x.IsGroup, () =>
        {
            RuleFor(x => x.ParticipantIds)
                .Must(ids => ids.Count == 1)
                .WithMessage("Direct chat requires exactly one participant.");
        });

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required.");

        RuleFor(x => x.ParticipantIds)
            .Must((cmd, ids) => !ids.Contains(cmd.RequesterId))
            .WithMessage("You cannot add yourself as a participant.");
    }
}