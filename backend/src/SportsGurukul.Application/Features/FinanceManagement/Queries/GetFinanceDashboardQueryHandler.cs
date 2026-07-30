using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetFinanceDashboardQueryHandler : IRequestHandler<GetFinanceDashboardQuery, Result<FinanceDashboardDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICouponRepository _couponRepository;

    public GetFinanceDashboardQueryHandler(IInvoiceRepository invoiceRepository, ICouponRepository couponRepository)
    {
        _invoiceRepository = invoiceRepository;
        _couponRepository = couponRepository;
    }

    public async Task<Result<FinanceDashboardDto>> Handle(GetFinanceDashboardQuery request, CancellationToken cancellationToken)
    {
        var allInvoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync(cancellationToken);
        var activeCoupons = await _couponRepository.GetActiveCouponsAsync(cancellationToken);

        var dto = new FinanceDashboardDto(
            TotalRevenue: allInvoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Total),
            OutstandingAmount: allInvoices.Sum(i => i.AmountDue),
            PendingInvoices: allInvoices.Count(i => i.Status == InvoiceStatus.Issued),
            OverdueInvoices: overdueInvoices.Count,
            RecentPayments: allInvoices.Count(i => i.Status == InvoiceStatus.Paid),
            WalletBalance: 0,
            ActiveCoupons: activeCoupons.Count,
            PendingRefunds: 0
        );

        return Result<FinanceDashboardDto>.Success(dto);
    }
}
