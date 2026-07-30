namespace SportsGurukul.Platform.FinancialReporting.Models;

public class ExportRequest
{
    public string ReportId { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public ReportFormat Format { get; set; } = ReportFormat.Excel;
    public Dictionary<string, string> Options { get; set; } = new();
}

public class ExportResult
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
