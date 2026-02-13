namespace Infrastructure.AmazonS3Storage;

public class AmazonS3StorageOptions
{
    public const string SectionName = "AmazonS3";

    public string BucketName { get; set; } = null!;
}