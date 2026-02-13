using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Contracts;

namespace Infrastructure.AmazonS3Storage;

public class AmazonS3StorageService(IAmazonS3 client, IOptions<AmazonS3StorageOptions> options) : IFileStorageService
{
    private readonly AmazonS3StorageOptions _options = options.Value;

    public async Task<string> UploadFileAsync(
        string fileName, 
        Stream fileStream, 
        MediaFolder mediaFolder, 
        CancellationToken ct)
    {
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid()}{extension}";
        
        var folderPath = mediaFolder switch
        {
            MediaFolder.ProfilePics => "profile-pics",
            MediaFolder.PostFiles => "posts",
            _ => "others"
        };

        var key = $"{folderPath}/{newFileName}";
        
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = GetContentType(extension)
        };

        try
        {
            var response = await client.PutObjectAsync(request, ct);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new FileStorageException($"S3 returned error: {response.HttpStatusCode}", null);
            }
            
            return key;
        }
        catch (AmazonS3Exception ex) 
        {
            throw new FileStorageException("Technical failure connecting to S3.", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string key, CancellationToken cancellationToken)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };

        try
        {
            var response = await client.DeleteObjectAsync(request, cancellationToken);

            return response.HttpStatusCode is System.Net.HttpStatusCode.NoContent or System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            throw new FileStorageException("Technical failure deleting file from S3.", ex);
        }
    }

    public string GetPresignedUrl(string key, int expirationMinutes)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
        };

        return client.GetPreSignedURL(request);
    }

    private string GetContentType(string extension) => extension.ToLower() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        _ => "application/octet-stream"
    };
}