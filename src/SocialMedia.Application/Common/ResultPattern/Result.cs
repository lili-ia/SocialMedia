namespace SocialMedia.Application.Common.ResultPattern;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorMessage { get; protected set; }
    public ErrorType? ErrorType { get; protected set; }

    protected Result()
    {
        IsSuccess = true;
    }

    protected Result(string errorMessage, ErrorType errorType)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result Success() => new Result();

    public static Result Failure(string errorMessage, ErrorType errorType = ResultPattern.ErrorType.Unknown)
        => new Result(errorMessage, errorType);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private Result(string errorMessage, ErrorType errorType) : base(errorMessage, errorType) { }

    public static Result<T> Success(T value) => new Result<T>(value);

    public new static Result<T> Failure(string errorMessage, ErrorType errorType = ResultPattern.ErrorType.Unknown)
        => new Result<T>(errorMessage, errorType);
}