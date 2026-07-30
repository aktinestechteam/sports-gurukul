using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class MarkInvoiceAsPaidCommandHandler : IRequestHandler<MarkInvoiceAsPaidCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceService _invoiceService;

    public MarkInvoiceAsPaidCommandHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<InvoiceDto>> Handle(MarkInvoiceAsPaidCommand request, CancellationToken cancellationToken)
    {
        return await _invoiceService.MarkAsPaidAsync(request.InvoiceId, cancellationToken);
    }
}
