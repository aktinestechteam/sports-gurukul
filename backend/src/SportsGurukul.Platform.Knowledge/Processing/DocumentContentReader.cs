using System.Text;
using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public static class DocumentContentReader
{
    public static async Task<byte[]> ReadBytesAsync(KnowledgeDocument document, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(document.StoragePath) && File.Exists(document.StoragePath))
        {
            return await File.ReadAllBytesAsync(document.StoragePath, ct);
        }

        if (!string.IsNullOrWhiteSpace(document.SourceUri)
            && Uri.TryCreate(document.SourceUri, UriKind.Absolute, out var uri)
            && uri.IsFile)
        {
            return await File.ReadAllBytesAsync(uri.LocalPath, ct);
        }

        throw new InvalidOperationException($"Document '{document.Id}' has no readable content source.");
    }

    public static string DecodeText(byte[] content, string? contentType = null)
    {
        var utf16 = contentType?.Contains("utf-16", StringComparison.OrdinalIgnoreCase) == true;
        var text = (utf16 ? Encoding.Unicode : Encoding.UTF8).GetString(content);
        return text.TrimStart('\uFEFF');
    }
}
