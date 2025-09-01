using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is not null
                ? new OkObjectResult(result.Value)
                : new NoContentResult();
        }

        return result switch
        {
            { IsSuccess: true, Value: not null } => new OkObjectResult(result.Value),
            { IsSuccess: true, Value: null } => new NoContentResult(),
            { IsSuccess: false, ErrorType: ErrorType.NotFound } => new NotFoundObjectResult(result.ErrorMessage),
            { IsSuccess: false, ErrorType: ErrorType.Validation } => new BadRequestObjectResult(result.ErrorMessage),
            { IsSuccess: false, ErrorType: ErrorType.Forbidden } => new ObjectResult(result.ErrorMessage) { StatusCode = 403 },
            { IsSuccess: false, ErrorType: ErrorType.Unauthorized } => new ObjectResult(result.ErrorMessage) { StatusCode = 401 },
            { IsSuccess: false, ErrorType: ErrorType.ServerError or null } => new ObjectResult(result.ErrorMessage ?? "Server error") { StatusCode = 500 },
            _ => new ObjectResult("Unexpected error") { StatusCode = 500 }
        };
    }
    
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        { 
            return new NoContentResult();
        }

        return result switch
        {
            { IsSuccess: false, ErrorType: ErrorType.NotFound } => new NotFoundObjectResult(result.ErrorMessage),
            { IsSuccess: false, ErrorType: ErrorType.Validation } => new BadRequestObjectResult(result.ErrorMessage),
            { IsSuccess: false, ErrorType: ErrorType.Forbidden } => new ObjectResult(result.ErrorMessage) { StatusCode = 403 },
            { IsSuccess: false, ErrorType: ErrorType.Unauthorized } => new ObjectResult(result.ErrorMessage) { StatusCode = 401 },
            { IsSuccess: false, ErrorType: ErrorType.ServerError or null } => new ObjectResult(result.ErrorMessage ?? "Server error") { StatusCode = 500 },
            _ => new ObjectResult("Unexpected error") { StatusCode = 500 }
        };
    }
}