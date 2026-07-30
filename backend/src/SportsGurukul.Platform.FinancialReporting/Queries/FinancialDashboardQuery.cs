using MediatR;
using SportsGurukul.Platform.FinancialReporting.Models;

namespace SportsGurukul.Platform.FinancialReporting.Queries;

public record FinancialDashboardQuery(ReportFilter? Filter = null) : IRequest<FinancialDashboard>;

public class FinancialDashboardQueryHandler : IRequestHandler<FinancialDashboardQuery, FinancialDashboard>
{
    private readonly Interfaces.IDashboardService _dashboardService;

    public FinancialDashboardQueryHandler(Interfaces.IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<FinancialDashboard> Handle(FinancialDashboardQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardService.GetDashboardAsync(request.Filter, cancellationToken);
    }
}
