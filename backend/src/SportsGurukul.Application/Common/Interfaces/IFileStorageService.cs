using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<FileStorageResult> UploadAsync(Stream fileStream, string fileName, string contentType, FileCategory category, CancellationToken cancellationToken = default);
    Task<Stream?> GetAsync(string storagePath, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
    string GetPublicUrl(string storagePath);
}

public class FileStorageResult
{
    public string StoredFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string? PublicUrl { get; set; }
    public long FileSize { get; set; }
}
