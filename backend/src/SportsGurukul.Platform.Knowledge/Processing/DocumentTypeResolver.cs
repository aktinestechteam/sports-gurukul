using SportsGurukul.Platform.Knowledge.Models;

namespace SportsGurukul.Platform.Knowledge.Processing;

public static class DocumentTypeResolver
{
    public static DocumentType FromContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return DocumentType.Unknown;
        }

        var ct = contentType.Trim().ToLowerInvariant();

        if (ct.Contains("pdf")) return DocumentType.Pdf;
        if (ct.Contains("wordprocessingml") || ct.Contains("msword")) return DocumentType.Word;
        if (ct.Contains("spreadsheetml") || ct.Contains("ms-excel")) return DocumentType.Excel;
        if (ct.Contains("presentationml") || ct.Contains("ms-powerpoint")) return DocumentType.PowerPoint;
        if (ct.Contains("markdown")) return DocumentType.Markdown;
        if (ct.Contains("html")) return DocumentType.Html;
        if (ct.Contains("csv")) return DocumentType.Csv;
        if (ct.Contains("json")) return DocumentType.Json;
        if (ct.Contains("xml")) return DocumentType.Xml;
        if (ct.StartsWith("image/")) return DocumentType.Image;
        if (ct.Contains("text")) return DocumentType.Text;

        return DocumentType.Unknown;
    }

    public static DocumentType FromFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return DocumentType.Unknown;
        }

        var ext = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();

        return ext switch
        {
            "pdf" => DocumentType.Pdf,
            "doc" or "docx" or "docm" => DocumentType.Word,
            "xls" or "xlsx" or "xlsm" => DocumentType.Excel,
            "ppt" or "pptx" or "pptm" => DocumentType.PowerPoint,
            "md" or "markdown" => DocumentType.Markdown,
            "htm" or "html" => DocumentType.Html,
            "txt" or "text" or "log" => DocumentType.Text,
            "csv" => DocumentType.Csv,
            "json" => DocumentType.Json,
            "xml" => DocumentType.Xml,
            "png" or "jpg" or "jpeg" or "gif" or "webp" or "tiff" or "bmp" or "svg" => DocumentType.Image,
            _ => DocumentType.Unknown
        };
    }
}
