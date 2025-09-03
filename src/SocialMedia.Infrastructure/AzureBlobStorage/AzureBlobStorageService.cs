using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Contracts;

namespace Infrastructure.AzureBlobStorage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageOptions _options;
    
    public AzureBlobStorageService(BlobServiceClient blobServiceClient, IOptions<AzureStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        
        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var blobClient = containerClient.GetBlobClient(uniqueName);

        await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string fullUri, CancellationToken cancellationToken)
    {
        var uri = new Uri(fullUri);
        var containerName = uri.Segments[1].TrimEnd('/');
        var blobName = string.Concat(uri.Segments.Skip(2));
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        
        bool deleted = await containerClient.DeleteBlobIfExistsAsync(blobName, cancellationToken: cancellationToken);

        return deleted;
    }
}