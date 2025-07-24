using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application;

namespace SocialMedia.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.Success)
        {
            return new OkObjectResult(new ApiResponse<T>
            {
                Success = true,
                Data = result.Value
            });
        }

        var errorResponse = new ApiResponse<T>
        {
            Success = false,
            Error = result.ErrorMessage ?? "Unknown error"
        };

        return result.ErrorType switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(errorResponse),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(errorResponse),
            ErrorType.Validation => new BadRequestObjectResult(errorResponse),
            ErrorType.Forbidden => new ObjectResult(errorResponse) { StatusCode = 403 },
            ErrorType.ServerError => new ObjectResult(errorResponse) { StatusCode = 500 },
            ErrorType.BadRequest => new BadRequestObjectResult(errorResponse),
            _ => new ObjectResult(new ApiResponse<T> { Success = false, Error = "Unexpected error" }) { StatusCode = 500 }
        };
    }
}