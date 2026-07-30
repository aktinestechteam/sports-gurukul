namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public enum ExportFormat
{
    Csv,
    Excel,
    Pdf
}

public enum ExportScope
{
    CurrentPage,
    AllResults,
    SelectedIds,
    DateRange
}

public record ExportRequest(
    ExportFormat Format,
    ExportScope Scope,
    string EntityType,
    List<string>? SelectedIds,
    DateTime? StartDate,
    DateTime? EndDate,
    Dictionary<string, string>? Filters,
    List<string>? IncludeColumns,
    Dictionary<string, object>? Options
);

public record ExportResult(
    Guid ExportId,
    ExportFormat Format,
    string EntityType,
    int TotalRecords,
    long FileSizeBytes,
    string FileName,
    string ContentType,
    byte[]? Data,
    DateTime GeneratedAt,
    long GenerationTimeMs
);

public record ExportOptionsDto(
    bool IncludeHeaders,
    string DateFormat,
    string NumberFormat,
    string? Culture,
    bool IncludeMetadata,
    string? SheetName,
    string? Title,
    bool Landscape,
    string? FontSize
);

public record ExportTemplateDto(
    string Name,
    ExportFormat Format,
    string EntityType,
    List<string> DefaultColumns,
    Dictionary<string, string> ColumnMappings,
    ExportOptionsDto Options
);

public record ExportHistoryDto(
    Guid Id,
    ExportFormat Format,
    string EntityType,
    int RecordCount,
    string Status,
    DateTime RequestedAt,
    DateTime? CompletedAt,
    long FileSizeBytes,
    string? Error
);

public interface IExportGenerator
{
    ExportFormat SupportedFormat { get; }
    Task<ExportResult> GenerateAsync<T>(IReadOnlyList<T> data, ExportRequest request, CancellationToken ct = default);
}

public interface ICsvExportGenerator : IExportGenerator { }
public interface IExcelExportGenerator : IExportGenerator { }
public interface IPdfExportGenerator : IExportGenerator { }
