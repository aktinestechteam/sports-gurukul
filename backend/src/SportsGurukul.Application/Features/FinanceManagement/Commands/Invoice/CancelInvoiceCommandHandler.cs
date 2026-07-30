using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceService _invoiceService;

    public CancelInvoiceCommandHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<InvoiceDto>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        return await _invoiceService.CancelInvoiceAsync(request.InvoiceId, request.Reason, cancellationToken);
    }
}
