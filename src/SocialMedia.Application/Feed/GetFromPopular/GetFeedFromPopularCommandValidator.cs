using FluentValidation;

namespace SocialMedia.Application.Feed.GetFromPopular;

public class GetFeedFromPopularCommandValidator : AbstractValidator<GetFeedFromPopularCommand>
{
    public GetFeedFromPopularCommandValidator()
    {
        RuleFor(x => x.ForUserId)
            .NotEmpty().WithMessage("ForUserId must be provided.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must be at most 100.");
    }
}