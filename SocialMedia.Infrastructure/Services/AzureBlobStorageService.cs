using Azure.Storage.Blobs;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Services;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobStorageService(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public string BaseUrl => _containerClient.Uri.ToString();

    public async Task UploadFileAsync(string fileName, Stream fileStream, CancellationToken ct)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: ct);
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken ct)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteAsync(cancellationToken: ct);
    }
}