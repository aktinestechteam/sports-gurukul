using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record RevenueSummaryQuery(DateTime FromDate, DateTime ToDate, ReportFilter? Filter = null) : IRequest<RevenueReport>;

public class RevenueSummaryQueryHandler : IRequestHandler<RevenueSummaryQuery, RevenueReport>
{
    private readonly Interfaces.IReportService _reportService;

    public RevenueSummaryQueryHandler(Interfaces.IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<RevenueReport> Handle(RevenueSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GenerateRevenueReportAsync(request.Filter, cancellationToken);
    }
}
