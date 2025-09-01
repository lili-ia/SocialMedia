using FluentValidation;

namespace SocialMedia.Application.Comments.Create;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must be provided.");
        
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("PostId must be provided.");
        
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text must be provided.")
            .MaximumLength(500).WithMessage("Text max length must not exceed 500 characters.");
    }
}