namespace SocialMedia.Shared.ResultPattern;

public enum ErrorType
{
    NotFound,
    Validation,
    ServerError,
    Unknown, 
    Unauthorized, 
    Forbidden,
    BadRequest,
    Conflict
}