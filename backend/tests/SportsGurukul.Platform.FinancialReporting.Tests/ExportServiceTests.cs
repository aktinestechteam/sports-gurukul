using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class ExportServiceTests
{
    private readonly IExportService _exportService;

    public ExportServiceTests()
    {
        var excelService = new StubExcelExportService();
        var csvService = new StubCsvExportService();
        var pdfService = new StubPdfExportService();
        _exportService = new ExportService(
            NullLogger<ExportService>.Instance, excelService, csvService, pdfService);
    }

    [Fact]
    public async Task ExportToExcel_ReturnsContent()
    {
        var data = new RevenueReport { TotalRevenue = 1000, TransactionCount = 10 };
        var result = await _exportService.ExportToExcelAsync(data, "revenue_test");
        Assert.True(result.Success);
        Assert.NotEmpty(result.FileContent);
        Assert.Equal("revenue_test.xlsx", result.FileName);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
    }

    [Fact]
    public async Task ExportToCsv_ReturnsContent()
    {
        var data = new List<RevenueLineItem>
        {
            new() { Date = DateTime.UtcNow, TransactionId = "TXN001", Amount = 100 },
            new() { Date = DateTime.UtcNow, TransactionId = "TXN002", Amount = 200 }
        };
        var result = await _exportService.ExportToCsvAsync(data, "revenue_test");
        Assert.True(result.Success);
        Assert.NotEmpty(result.FileContent);
        Assert.Equal("revenue_test.csv", result.FileName);
    }

    [Fact]
    public async Task ExportToPdf_ReturnsContent()
    {
        var data = new RevenueReport { TotalRevenue = 1000, TransactionCount = 10 };
        var result = await _exportService.ExportToPdfAsync(data, "revenue_test");
        Assert.True(result.Success);
        Assert.NotEmpty(result.FileContent);
        Assert.Equal("revenue_test.pdf", result.FileName);
    }

    [Fact]
    public async Task ExportAsync_ExcelFormat_Works()
    {
        var data = new FinancialDashboard();
        var result = await _exportService.ExportAsync(data, ReportFormat.Excel, "dashboard");
        Assert.True(result.Success);
        Assert.EndsWith(".xlsx", result.FileName);
    }

    [Fact]
    public async Task ExportAsync_CsvFormat_Works()
    {
        var data = new FinancialDashboard();
        var result = await _exportService.ExportAsync(data, ReportFormat.Csv, "dashboard");
        Assert.True(result.Success);
        Assert.EndsWith(".csv", result.FileName);
    }

    [Fact]
    public async Task ExportAsync_PdfFormat_Works()
    {
        var data = new FinancialDashboard();
        var result = await _exportService.ExportAsync(data, ReportFormat.Pdf, "dashboard");
        Assert.True(result.Success);
        Assert.EndsWith(".pdf", result.FileName);
    }

    [Fact]
    public async Task StubExcelService_GeneratesContent()
    {
        var service = new StubExcelExportService();
        var result = await service.GenerateExcelAsync(new RevenueReport { TotalRevenue = 500 });
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task StubCsvService_GeneratesContent()
    {
        var service = new StubCsvExportService();
        var result = await service.GenerateCsvAsync(new List<RevenueLineItem>());
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task StubPdfService_GeneratesContent()
    {
        var service = new StubPdfExportService();
        var result = await service.GeneratePdfAsync(new RevenueReport(), "Test");
        Assert.NotEmpty(result);
    }
}
