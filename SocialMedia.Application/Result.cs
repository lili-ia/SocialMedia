namespace SocialMedia.Application;

public class Result<T>
{
    public T? Value { get; }
    public bool Success { get; }
    public string? ErrorMessage { get; }

    private Result(T value)
    {
        Value = value;
        Success = true;
    }

    private Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Success = false;
    }

    public static Result<T> SuccessResult(T value) => new(value);
    public static Result<T> FailureResult(string errorMessage) => new(errorMessage);
}