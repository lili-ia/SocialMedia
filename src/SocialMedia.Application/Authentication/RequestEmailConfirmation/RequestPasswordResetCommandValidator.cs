using FluentValidation;

namespace SocialMedia.Application.Authentication.RequestEmailConfirmation;

public class RequestPasswordResetCommandValidator : AbstractValidator<RequestEmailConfirmationCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100);
    }
}
