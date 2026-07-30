using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetOutstandingInvoicesQueryHandler : IRequestHandler<GetOutstandingInvoicesQuery, Result<IReadOnlyList<InvoiceSummaryDto>>>
{
    private readonly IInvoiceService _invoiceService;

    public GetOutstandingInvoicesQueryHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<IReadOnlyList<InvoiceSummaryDto>>> Handle(GetOutstandingInvoicesQuery request, CancellationToken cancellationToken)
    {
        return await _invoiceService.GetOutstandingInvoicesAsync(cancellationToken);
    }
}
