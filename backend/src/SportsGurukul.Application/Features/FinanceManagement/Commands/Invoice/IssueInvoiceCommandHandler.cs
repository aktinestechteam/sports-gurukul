using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceService _invoiceService;

    public IssueInvoiceCommandHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<InvoiceDto>> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
    {
        return await _invoiceService.IssueInvoiceAsync(request.InvoiceId, cancellationToken);
    }
}
