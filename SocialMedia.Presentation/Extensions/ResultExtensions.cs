using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application;

namespace SocialMedia.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        return result switch
        {
            { Success: true, Value: not null } => new OkObjectResult(result.Value),
            { Success: true, Value: null } => new NoContentResult(),
            { ErrorType: ErrorType.NotFound } => new NotFoundObjectResult(result.ErrorMessage),
            { ErrorType: ErrorType.Validation } => new BadRequestObjectResult(result.ErrorMessage),
            { ErrorType: ErrorType.Forbidden } => new ObjectResult(result.ErrorMessage) { StatusCode = 403 },
            { ErrorType: ErrorType.ServerError } => new ObjectResult(result.ErrorMessage) { StatusCode = 500 },
            _ => new ObjectResult("Unexpected error") { StatusCode = 500 }
        };
    }
}