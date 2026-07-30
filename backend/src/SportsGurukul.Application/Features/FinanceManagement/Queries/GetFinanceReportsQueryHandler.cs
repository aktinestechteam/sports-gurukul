using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetFinanceReportsQueryHandler : IRequestHandler<GetFinanceReportsQuery, Result<FinanceReportDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetFinanceReportsQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<FinanceReportDto>> Handle(GetFinanceReportsQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var paid = invoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();

        var dto = new FinanceReportDto(
            TotalRevenue: paid.Sum(i => i.Total),
            TotalExpenses: 0,
            NetIncome: paid.Sum(i => i.Total),
            TotalTaxCollected: paid.Sum(i => i.TaxTotal),
            TotalDiscountGiven: paid.Sum(i => i.DiscountTotal),
            TotalInvoicesIssued: invoices.Count,
            TotalPaymentsReceived: paid.Count,
            TotalRefundsProcessed: 0,
            OutstandingReceivables: invoices.Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid).Sum(i => i.AmountDue),
            RevenueByCategory: new Dictionary<string, decimal>()
        );

        return Result<FinanceReportDto>.Success(dto);
    }
}
