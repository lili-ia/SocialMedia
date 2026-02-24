using FluentValidation;
using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any())
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errorMessages = failures
                .Select(x => x.ErrorMessage)
                .ToList();

            if (typeof(TResponse).IsGenericType)
            {
                return (TResponse)(object)
                    Result.Failure(string.Join(", ", errorMessages), ErrorType.Validation);
            }

            throw new ValidationException(failures);
        }

        return await next(ct);
    }
}