using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record GetInvoiceReceiptQuery(Guid InvoiceId) : IRequest<Result<InvoiceReceiptDto>>;

public record InvoiceReceiptDto(
    Guid Id,
    string InvoiceNumber,
    DateTime IssueDate,
    string? AthleteName,
    string? AcademyName,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal AmountPaid,
    string Currency,
    List<ReceiptLineItemDto> LineItems
);

public record ReceiptLineItemDto(string Description, int Quantity, decimal UnitPrice, decimal Total);
