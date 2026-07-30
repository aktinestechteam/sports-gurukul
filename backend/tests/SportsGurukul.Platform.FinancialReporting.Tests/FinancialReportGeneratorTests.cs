using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.FinancialReporting.Exports;
using SportsGurukul.Platform.FinancialReporting.Interfaces;
using SportsGurukul.Platform.FinancialReporting.Models;
using SportsGurukul.Platform.FinancialReporting.Reports;

namespace SportsGurukul.Platform.FinancialReporting.Tests;

public class FinancialReportGeneratorTests
{
    private readonly IFinancialReportGenerator _generator;

    public FinancialReportGeneratorTests()
    {
        var reportService = new ReportService(NullLogger<ReportService>.Instance);
        var excelService = new StubExcelExportService();
        var csvService = new StubCsvExportService();
        var pdfService = new StubPdfExportService();
        var exportService = new ExportService(
            NullLogger<ExportService>.Instance, excelService, csvService, pdfService);
        _generator = new FinancialReportGenerator(
            reportService, exportService, NullLogger<FinancialReportGenerator>.Instance);
    }

    [Fact]
    public async Task GenerateReport_Revenue_ReturnsResult()
    {
        var request = new ReportRequest
        {
            Type = ReportType.Revenue,
            FromDate = DateTime.UtcNow.AddMonths(-1),
            ToDate = DateTime.UtcNow
        };
        var result = await _generator.GenerateReportAsync(request);
        Assert.NotNull(result);
        Assert.Equal(ReportType.Revenue, result.Type);
    }

    [Fact]
    public async Task GenerateReport_AllTypes_ReturnResult()
    {
        foreach (ReportType type in Enum.GetValues<ReportType>())
        {
            var request = new ReportRequest
            {
                Type = type,
                FromDate = DateTime.UtcNow.AddMonths(-1),
                ToDate = DateTime.UtcNow
            };
            var result = await _generator.GenerateReportAsync(request);
            Assert.NotNull(result);
            Assert.Equal(type, result.Type);
        }
    }

    [Fact]
    public async Task GenerateAndExport_Excel_ReturnsFile()
    {
        var request = new ReportRequest
        {
            Type = ReportType.Revenue,
            FromDate = DateTime.UtcNow.AddMonths(-1),
            ToDate = DateTime.UtcNow,
            Format = ReportFormat.Excel
        };
        var result = await _generator.GenerateAndExportAsync(request);
        Assert.NotNull(result);
        Assert.NotEmpty(result.FileName);
    }
}
