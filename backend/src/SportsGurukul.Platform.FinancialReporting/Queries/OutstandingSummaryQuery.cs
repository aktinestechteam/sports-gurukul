using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record OutstandingSummaryQuery(ReportFilter? Filter = null) : IRequest<OutstandingInvoicesReport>;

public class OutstandingSummaryQueryHandler : IRequestHandler<OutstandingSummaryQuery, OutstandingInvoicesReport>
{
    private readonly Interfaces.IReportService _reportService;

    public OutstandingSummaryQueryHandler(Interfaces.IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<OutstandingInvoicesReport> Handle(OutstandingSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GenerateOutstandingInvoicesReportAsync(request.Filter, cancellationToken);
    }
}
