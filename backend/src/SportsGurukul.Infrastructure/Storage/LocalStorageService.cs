using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Storage;

public class LocalStorageService : IFileStorageService
{
    private readonly StorageOptions _options;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(IOptions<StorageOptions> options, ILogger<LocalStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FileStorageResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        FileCategory category,
        CancellationToken cancellationToken = default)
    {
        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var folder = GetFolder(category);
        var fullPath = Path.Combine(_options.BasePath, folder);

        Directory.CreateDirectory(fullPath);

        var filePath = Path.Combine(fullPath, storedFileName);

        await using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        var fileSize = fileStreamOutput.Length;

        _logger.LogInformation("File uploaded locally: {FilePath} ({FileSize} bytes)", filePath, fileSize);

        var publicUrl = !string.IsNullOrEmpty(_options.BaseUrl)
            ? $"{_options.BaseUrl.TrimEnd('/')}/{_options.BasePath.Trim('/')}/{folder}/{storedFileName}"
            : null;

        return new FileStorageResult
        {
            StoredFileName = storedFileName,
            StoragePath = filePath,
            PublicUrl = publicUrl,
            FileSize = fileSize
        };
    }

    public Task<Stream?> GetAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
        {
            _logger.LogWarning("File not found: {StoragePath}", storagePath);
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(storagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(storagePath))
        {
            _logger.LogWarning("File not found for deletion: {StoragePath}", storagePath);
            return Task.FromResult(false);
        }

        File.Delete(storagePath);
        _logger.LogInformation("File deleted locally: {StoragePath}", storagePath);
        return Task.FromResult(true);
    }

    public string GetPublicUrl(string storagePath)
    {
        if (!string.IsNullOrEmpty(_options.BaseUrl))
        {
            var relativePath = Path.GetRelativePath(_options.BasePath, storagePath)
                .Replace('\\', '/');
            return $"{_options.BaseUrl.TrimEnd('/')}/{_options.BasePath.Trim('/')}/{relativePath}";
        }

        return storagePath;
    }

    private static string GetFolder(FileCategory category) => category switch
    {
        FileCategory.Image => "images",
        FileCategory.Document => "documents",
        FileCategory.Video => "videos",
        _ => "other"
    };
}
