using FluentValidation.Results;

namespace SocialMedia.Application.Common.ResultPattern;

public static class ValidationResultExtensions
{
    public static Result<T> ToFailureResult<T>(this ValidationResult validationResult)
    {
        var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
        return Result<T>.Failure(errorMessages, ErrorType.Validation);
    }
    
    public static Result ToFailureResult(this ValidationResult validationResult)
    {
        var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
        return Result.Failure(errorMessages, ErrorType.Validation);
    }
}