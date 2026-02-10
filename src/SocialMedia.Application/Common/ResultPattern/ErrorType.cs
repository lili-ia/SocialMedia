namespace SocialMedia.Application.Common.ResultPattern;

public enum ErrorType
{
    NotFound,
    Validation,
    ServerError,
    Unknown, 
    Unauthorized, 
    Forbidden,
    BadRequest,
    Conflict,
    TooManyRequests
}