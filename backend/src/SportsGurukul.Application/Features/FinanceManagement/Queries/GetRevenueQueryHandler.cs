using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetRevenueQueryHandler : IRequestHandler<GetRevenueQuery, Result<RevenueDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetRevenueQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<RevenueDto>> Handle(GetRevenueQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var paid = invoices.Where(i => i.Status == InvoiceStatus.Paid);

        if (request.FromDate.HasValue)
            paid = paid.Where(i => i.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            paid = paid.Where(i => i.CreatedAt <= request.ToDate.Value);

        var paidList = paid.ToList();
        var dto = new RevenueDto(
            TotalRevenue: paidList.Sum(i => i.Total),
            TotalTax: paidList.Sum(i => i.TaxTotal),
            TotalDiscount: paidList.Sum(i => i.DiscountTotal),
            NetRevenue: paidList.Sum(i => i.Total - i.TaxTotal),
            InvoiceCount: paidList.Count,
            PaymentCount: 0
        );

        return Result<RevenueDto>.Success(dto);
    }
}
