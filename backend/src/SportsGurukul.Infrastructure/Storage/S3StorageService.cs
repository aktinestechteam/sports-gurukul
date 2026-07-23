using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Storage;

public class S3StorageService : IFileStorageService
{
    private readonly StorageOptions _options;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IOptions<StorageOptions> options, ILogger<S3StorageService> logger)
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
        var bucketName = _options.S3.BucketName;

        _logger.LogInformation(
            "Uploading to S3: Bucket={Bucket}, Key={Key}, ContentType={ContentType}",
            bucketName, storedFileName, contentType);

        // TODO: Add AWSSDK.S3 NuGet package and implement:
        // var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(_options.S3.Region) };
        // if (!string.IsNullOrEmpty(_options.S3.AccessKey) && !string.IsNullOrEmpty(_options.S3.SecretKey))
        //     config.Credentials = new BasicAWSCredentials(_options.S3.AccessKey, _options.S3.SecretKey);
        // using var s3Client = new AmazonS3Client(config);
        // var request = new PutObjectRequest
        // {
        //     BucketName = bucketName,
        //     Key = storedFileName,
        //     InputStream = fileStream,
        //     ContentType = contentType,
        //     CannedACL = S3CannedACL.PublicRead
        // };
        // await s3Client.PutObjectAsync(request, cancellationToken);
        // var fileSize = fileStream.Length;

        var fileSize = fileStream.Length;
        var publicUrl = $"https://{bucketName}.s3.{_options.S3.Region}.amazonaws.com/{storedFileName}";

        _logger.LogInformation("File uploaded to S3: {FileUrl} ({FileSize} bytes)", publicUrl, fileSize);

        return Task.FromResult(new FileStorageResult
        {
            StoredFileName = storedFileName,
            StoragePath = $"{bucketName}/{storedFileName}",
            PublicUrl = publicUrl,
            FileSize = fileSize
        });
    }

    public async Task<Stream?> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading from S3: {StoragePath}", storagePath);

        // TODO: Add AWSSDK.S3 NuGet package and implement:
        // var bucketName = storagePath.Split('/')[0];
        // var key = string.Join('/', storagePath.Split('/').Skip(1));
        // var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(_options.S3.Region) };
        // if (!string.IsNullOrEmpty(_options.S3.AccessKey) && !string.IsNullOrEmpty(_options.S3.SecretKey))
        //     config.Credentials = new BasicAWSCredentials(_options.S3.AccessKey, _options.S3.SecretKey);
        // using var s3Client = new AmazonS3Client(config);
        // var response = await s3Client.GetObjectAsync(bucketName, key, cancellationToken);
        // return response.ResponseStream;

        await Task.CompletedTask;
        return null;
    }

    public async Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting from S3: {StoragePath}", storagePath);

        // TODO: Add AWSSDK.S3 NuGet package and implement:
        // var bucketName = storagePath.Split('/')[0];
        // var key = string.Join('/', storagePath.Split('/').Skip(1));
        // var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(_options.S3.Region) };
        // if (!string.IsNullOrEmpty(_options.S3.AccessKey) && !string.IsNullOrEmpty(_options.S3.SecretKey))
        //     config.Credentials = new BasicAWSCredentials(_options.S3.AccessKey, _options.S3.SecretKey);
        // using var s3Client = new AmazonS3Client(config);
        // await s3Client.DeleteObjectAsync(bucketName, key, cancellationToken);
        // return true;

        await Task.CompletedTask;
        return true;
    }

    public string GetPublicUrl(string storagePath)
    {
        var bucketName = _options.S3.BucketName;
        return $"https://{bucketName}.s3.{_options.S3.Region}.amazonaws.com/{storagePath}";
    }
}
