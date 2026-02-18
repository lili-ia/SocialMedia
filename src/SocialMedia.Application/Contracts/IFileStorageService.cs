
using SocialMedia.Application.Common;

namespace SocialMedia.Application.Contracts;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, MediaFolder mediaFolder, CancellationToken ct);

    Task<bool> DeleteFileAsync(string key, CancellationToken ct);

    string GetPresignedUrl(string key, int expirationMinutes = 60);
}