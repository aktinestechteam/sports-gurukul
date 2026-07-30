using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record LedgerQuery(DateTime FromDate, DateTime ToDate, ReportFilter? Filter = null) : IRequest<LedgerReport>;

public class LedgerQueryHandler : IRequestHandler<LedgerQuery, LedgerReport>
{
    private readonly Interfaces.IReportService _reportService;

    public LedgerQueryHandler(Interfaces.IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<LedgerReport> Handle(LedgerQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GenerateLedgerReportAsync(request.Filter, cancellationToken);
    }
}
