namespace SportsGurukul.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public StorageProvider Provider { get; set; } = StorageProvider.Local;
    public string BasePath { get; set; } = "uploads";
    public string? BaseUrl { get; set; }
    public AzureStorageOptions Azure { get; set; } = new();
    public S3StorageOptions S3 { get; set; } = new();
}

public enum StorageProvider
{
    Local = 0,
    Azure = 1,
    S3 = 2
}

public sealed class AzureStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "user-files";
}

public sealed class S3StorageOptions
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
}
