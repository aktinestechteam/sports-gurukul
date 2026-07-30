using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record SettlementSummaryQuery(DateTime FromDate, DateTime ToDate, ReportFilter? Filter = null) : IRequest<SettlementReport>;

public class SettlementSummaryQueryHandler : IRequestHandler<SettlementSummaryQuery, SettlementReport>
{
    private readonly Interfaces.IReportService _reportService;

    public SettlementSummaryQueryHandler(Interfaces.IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<SettlementReport> Handle(SettlementSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GenerateSettlementReportAsync(request.Filter, cancellationToken);
    }
}
