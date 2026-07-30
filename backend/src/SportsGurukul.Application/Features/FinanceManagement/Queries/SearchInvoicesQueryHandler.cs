using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class SearchInvoicesQueryHandler : IRequestHandler<SearchInvoicesQuery, Result<IReadOnlyList<InvoiceSummaryDto>>>
{
    private readonly IInvoiceService _invoiceService;

    public SearchInvoicesQueryHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<IReadOnlyList<InvoiceSummaryDto>>> Handle(SearchInvoicesQuery request, CancellationToken cancellationToken)
    {
        var searchRequest = new InvoiceSearchRequest(
            request.SearchTerm,
            request.Status,
            request.AthleteId,
            request.AcademyId,
            request.FromDate,
            request.ToDate,
            request.Page,
            request.PageSize
        );
        return await _invoiceService.SearchInvoicesAsync(searchRequest, cancellationToken);
    }
}
