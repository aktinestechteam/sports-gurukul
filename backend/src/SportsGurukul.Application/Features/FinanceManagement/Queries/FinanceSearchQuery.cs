using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FinanceManagement.Queries;

public record FinanceSearchQuery(string? SearchTerm, string? EntityType, DateTime? FromDate, DateTime? ToDate, int Page, int PageSize) : IRequest<Result<FinanceSearchResultDto>>;

public record FinanceSearchResultDto(
    IReadOnlyList<InvoiceSearchHit> Invoices,
    IReadOnlyList<PaymentSearchHit> Payments,
    IReadOnlyList<RefundSearchHit> Refunds,
    int TotalResults
);

public record InvoiceSearchHit(Guid Id, string InvoiceNumber, decimal Amount, string Status, DateTime CreatedAt);
public record PaymentSearchHit(Guid Id, string PaymentReference, decimal Amount, string Status, DateTime CreatedAt);
public record RefundSearchHit(Guid Id, string RefundNumber, decimal Amount, string Status, DateTime CreatedAt);
