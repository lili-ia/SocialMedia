namespace SocialMedia;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    
    public T? Data { get; init; }
    
    public string? Error { get; init; }
}