using FluentValidation;

namespace SocialMedia.Application.Users.Search;

public class SearchUsersCommandValidator : AbstractValidator<SearchUsersCommand>
{
    public SearchUsersCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username must be provided.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must be at most 100.");
    }
}