
namespace SocialMedia.Application.Contracts;

public interface IFileStorageService
{
    string BaseUrl { get; }
    
    Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken ct);

    Task DeleteFileAsync(string fileName, CancellationToken ct);
}