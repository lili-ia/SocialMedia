using FluentValidation;

namespace SocialMedia.Application.Users.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] AllowedExtensions = [".jpeg", ".jpg", ".png"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private const int MinimumAge = 13;
    
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId must be provided.");
        
        RuleFor(x => x)
            .Must(x => 
                !string.IsNullOrWhiteSpace(x.Bio) 
                || x.ProfilePic is not null 
                || x.BirthDate is not null)
            .WithMessage("Either Bio, ProfilePic or BirthDate must be provided.");

        When(x => !string.IsNullOrWhiteSpace(x.Bio), () =>
        {
            RuleFor(x => x.Bio)
                .MaximumLength(300).WithMessage("Bio max length must not exceed 300 characters.");
        });
        
        When(x => x.ProfilePic is not null, () =>
        {
            RuleFor(x => x.ProfilePic).ChildRules(file =>
            {
                file.RuleFor(f => f.FileName)
                    .Must(f => AllowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .WithMessage(f => $"File '{f.FileName} must be a photo (.jpeg, .jpg, .png).'");
                
                file.RuleFor(f => f.Content.Length)
                    .LessThanOrEqualTo(MaxFileSizeBytes)
                    .WithMessage(f => $"File '{f.FileName}' exceeds the maximum allowed size of 5 MB.");
            });
        });
        
        When(x => x.BirthDate is not null, () =>
        {
            RuleFor(x => x.BirthDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("BirthDate cannot be in the future.")
                .Must(date => date <= DateTime.UtcNow.AddYears(-MinimumAge))
                .WithMessage($"User must be at least {MinimumAge} years old.");
        });
    }
}