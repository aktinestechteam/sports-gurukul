using MediatR;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public class GetInvoiceReceiptQueryHandler : IRequestHandler<GetInvoiceReceiptQuery, Result<InvoiceReceiptDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceReceiptQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<InvoiceReceiptDto>> Handle(GetInvoiceReceiptQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
            return Result<InvoiceReceiptDto>.Failure("Invoice not found");

        var dto = new InvoiceReceiptDto(
            invoice.Id,
            invoice.InvoiceNumber ?? string.Empty,
            invoice.IssueDate,
            null,
            null,
            invoice.SubTotal,
            invoice.TaxTotal,
            invoice.DiscountTotal,
            invoice.Total,
            invoice.AmountPaid,
            invoice.Currency,
            invoice.Items.Select(i => new ReceiptLineItemDto(
                i.Description, i.Quantity, i.UnitPrice, i.TotalAmount
            )).ToList()
        );

        return Result<InvoiceReceiptDto>.Success(dto);
    }
}
