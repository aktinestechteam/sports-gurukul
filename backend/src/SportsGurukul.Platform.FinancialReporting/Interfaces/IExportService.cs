using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Interfaces;

public interface IExportService
{
    Task<ExportResult> ExportAsync<T>(T data, ReportFormat format, string fileName, CancellationToken cancellationToken = default);
    Task<ExportResult> ExportToExcelAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
    Task<ExportResult> ExportToCsvAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
    Task<ExportResult> ExportToPdfAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
}

public interface IExcelExportService
{
    Task<byte[]> GenerateExcelAsync<T>(T data, string sheetName = "Report");
}

public interface ICsvExportService
{
    Task<byte[]> GenerateCsvAsync<T>(T data);
}

public interface IPdfExportService
{
    Task<byte[]> GeneratePdfAsync<T>(T data, string title = "Report");
}
