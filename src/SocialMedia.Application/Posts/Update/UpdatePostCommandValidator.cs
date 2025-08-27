using FluentValidation;

namespace SocialMedia.Application.Posts.Update;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("PostId must be provided.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must be provided.");
        
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Text must be provided.")
            .MaximumLength(2000).WithMessage("Text max length must not exceed 2000 characters.");
    }
}