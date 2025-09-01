using FluentValidation;

namespace SocialMedia.Application.Posts.GetMyInactive;

public class GetMyInactivePostsCommandValidator : AbstractValidator<GetMyInactivePostsCommand>
{
    public GetMyInactivePostsCommandValidator()
    {
        RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId must be provided.");
        
        RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size must be at most 100.");
    }
}