namespace SocialMedia.Application.Posts;

public sealed record FileData(string FileName, Stream Content) : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await Content.DisposeAsync();
    }
}