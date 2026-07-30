using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceService _invoiceService;

    public CreateInvoiceCommandHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<Result<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateInvoiceRequest(
            request.AthleteId,
            request.AcademyId,
            request.Description,
            request.DueDate,
            request.Currency,
            request.LineItems,
            request.CouponCode,
            request.ScholarshipId
        );
        return await _invoiceService.CreateInvoiceAsync(createRequest, cancellationToken);
    }
}
