using System.Text;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Exports;

public class ExportService : IExportService
{
    private readonly ILogger<ExportService> _logger;
    private readonly IExcelExportService _excelService;
    private readonly ICsvExportService _csvService;
    private readonly IPdfExportService _pdfService;

    public ExportService(
        ILogger<ExportService> logger,
        IExcelExportService excelService,
        ICsvExportService csvService,
        IPdfExportService pdfService)
    {
        _logger = logger;
        _excelService = excelService;
        _csvService = csvService;
        _pdfService = pdfService;
    }

    public async Task<ExportResult> ExportAsync<T>(T data, ReportFormat format, string fileName, CancellationToken cancellationToken = default)
    {
        return format switch
        {
            ReportFormat.Excel => await ExportToExcelAsync(data, fileName, cancellationToken),
            ReportFormat.Csv => await ExportToCsvAsync(data, fileName, cancellationToken),
            ReportFormat.Pdf => await ExportToPdfAsync(data, fileName, cancellationToken),
            _ => await ExportToExcelAsync(data, fileName, cancellationToken)
        };
    }

    public async Task<ExportResult> ExportToExcelAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting {Type} to Excel: {FileName}", typeof(T).Name, fileName);
        var content = await _excelService.GenerateExcelAsync(data);
        return CreateResult(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
    }

    public async Task<ExportResult> ExportToCsvAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting {Type} to CSV: {FileName}", typeof(T).Name, fileName);
        var content = await _csvService.GenerateCsvAsync(data);
        return CreateResult(content, "text/csv", $"{fileName}.csv");
    }

    public async Task<ExportResult> ExportToPdfAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting {Type} to PDF: {FileName}", typeof(T).Name, fileName);
        var content = await _pdfService.GeneratePdfAsync(data);
        return CreateResult(content, "application/pdf", $"{fileName}.pdf");
    }

    private static ExportResult CreateResult(byte[] content, string contentType, string fileName)
    {
        return new ExportResult
        {
            FileContent = content, ContentType = contentType, FileName = fileName,
            FileSize = content.Length, Success = true
        };
    }
}

public class StubExcelExportService : IExcelExportService
{
    public Task<byte[]> GenerateExcelAsync<T>(T data, string sheetName = "Report")
    {
        var csv = SerializeToCsv(data);
        var html = $"<html><body><table>{csv}</table></body></html>";
        return Task.FromResult(Encoding.UTF8.GetBytes(html));
    }

    private static string SerializeToCsv<T>(T data)
    {
        if (data is null) return string.Empty;
        var lines = new List<string>();
        var type = data.GetType();
        if (data is System.Collections.IEnumerable enumerable && data is not string)
        {
            var elementType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)
                ? type.GetGenericArguments()[0]
                : type.GetElementType() ?? typeof(object);
            var props = elementType.GetProperties();
            var header = string.Join(",", props.Select(p => p.Name));
            lines.Add(header);
            foreach (var item in enumerable)
            {
                lines.Add(string.Join(",", props.Select(p => p.GetValue(item)?.ToString() ?? "")));
            }
        }
        else
        {
            var props = type.GetProperties();
            lines.Add(string.Join(",", props.Select(p => p.Name)));
            lines.Add(string.Join(",", props.Select(p => p.GetValue(data)?.ToString() ?? "")));
        }
        return string.Join("\n", lines);
    }
}

public class StubCsvExportService : ICsvExportService
{
    public Task<byte[]> GenerateCsvAsync<T>(T data)
    {
        var csv = SerializeToCsv(data);
        return Task.FromResult(Encoding.UTF8.GetBytes(csv));
    }

    private static string SerializeToCsv<T>(T data)
    {
        if (data is null) return string.Empty;
        var lines = new List<string>();

        if (data is System.Collections.IEnumerable enumerable && data is not string)
        {
            var elementType = typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>)
                ? typeof(T).GetGenericArguments()[0]
                : typeof(T).GetElementType() ?? typeof(object);
            var props = elementType.GetProperties();
            var header = string.Join(",", props.Select(p => p.Name));
            lines.Add(header);
            foreach (var item in enumerable)
            {
                lines.Add(string.Join(",", props.Select(p => p.GetValue(item)?.ToString() ?? "")));
            }
        }
        else
        {
            var props = typeof(T).GetProperties();
            lines.Add(string.Join(",", props.Select(p => p.Name)));
            lines.Add(string.Join(",", props.Select(p => p.GetValue(data)?.ToString() ?? "")));
        }

        return string.Join("\n", lines);
    }
}

public class StubPdfExportService : IPdfExportService
{
    public Task<byte[]> GeneratePdfAsync<T>(T data, string title = "Report")
    {
        var content = $"PDF Report: {title}\nGenerated: {DateTime.UtcNow:O}\n\n{System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}";
        return Task.FromResult(Encoding.UTF8.GetBytes(content));
    }
}
