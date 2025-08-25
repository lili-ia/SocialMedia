namespace Infrastructure.AzureBlobStorage;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; } = null!;

    public string ContainerName { get; set; } = null!;
}