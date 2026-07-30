using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record TaxSummaryQuery(DateTime FromDate, DateTime ToDate, ReportFilter? Filter = null) : IRequest<TaxReport>;

public class TaxSummaryQueryHandler : IRequestHandler<TaxSummaryQuery, TaxReport>
{
    private readonly Interfaces.IReportService _reportService;

    public TaxSummaryQueryHandler(Interfaces.IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<TaxReport> Handle(TaxSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _reportService.GenerateTaxReportAsync(request.Filter, cancellationToken);
    }
}
