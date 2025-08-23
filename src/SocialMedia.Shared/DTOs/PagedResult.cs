namespace SocialMedia.Shared.DTOs;

public class PagedResult<T>
{
    public int TotalCount { get; set; } = 0;

    public List<T> Items { get; set; } = [];
}