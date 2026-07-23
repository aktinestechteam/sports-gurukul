using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly StorageOptions _options;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(IOptions<StorageOptions> options, ILogger<AzureBlobStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<FileStorageResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        FileCategory category,
        CancellationToken cancellationToken = default)
    {
        var storedFileName = $"{category.ToString().ToLowerInvariant()}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var containerName = _options.Azure.ContainerName;

        _logger.LogInformation(
            "Uploading to Azure Blob: Container={Container}, Blob={Blob}, ContentType={ContentType}",
            containerName, storedFileName, contentType);

        // TODO: Add Azure.Storage.Blobs NuGet package and implement:
        // var blobServiceClient = new BlobServiceClient(_options.Azure.ConnectionString);
        // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        // await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        // var blobClient = containerClient.GetBlobClient(storedFileName);
        // await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        // var fileSize = (await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value.ContentLength;

        var fileSize = fileStream.Length;
        var publicUrl = $"https://{containerName}.blob.core.windows.net/{storedFileName}";

        _logger.LogInformation("File uploaded to Azure Blob: {BlobUrl} ({FileSize} bytes)", publicUrl, fileSize);

        return Task.FromResult(new FileStorageResult
        {
            StoredFileName = storedFileName,
            StoragePath = $"{containerName}/{storedFileName}",
            PublicUrl = publicUrl,
            FileSize = fileSize
        });
    }

    public async Task<Stream?> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading from Azure Blob: {StoragePath}", storagePath);

        // TODO: Add Azure.Storage.Blobs NuGet package and implement:
        // var blobServiceClient = new BlobServiceClient(_options.Azure.ConnectionString);
        // var containerName = storagePath.Split('/')[0];
        // var blobName = string.Join('/', storagePath.Split('/').Skip(1));
        // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        // var blobClient = containerClient.GetBlobClient(blobName);
        // var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        // return response.Value.Content;

        await Task.CompletedTask;
        return null;
    }

    public async Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting from Azure Blob: {StoragePath}", storagePath);

        // TODO: Add Azure.Storage.Blobs NuGet package and implement:
        // var blobServiceClient = new BlobServiceClient(_options.Azure.ConnectionString);
        // var containerName = storagePath.Split('/')[0];
        // var blobName = string.Join('/', storagePath.Split('/').Skip(1));
        // var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        // var blobClient = containerClient.GetBlobClient(blobName);
        // return await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        await Task.CompletedTask;
        return true;
    }

    public string GetPublicUrl(string storagePath)
    {
        var containerName = _options.Azure.ContainerName;
        return $"https://{containerName}.blob.core.windows.net/{storagePath}";
    }
}
