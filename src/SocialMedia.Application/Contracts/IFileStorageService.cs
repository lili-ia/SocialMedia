
namespace SocialMedia.Application.Contracts;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken);

    Task<bool> DeleteFileAsync(string fullUri, CancellationToken cancellationToken);
}