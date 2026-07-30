using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    Guid? AthleteId,
    Guid? AcademyId,
    string? AthleteName,
    string? AcademyName,
    string? Description,
    decimal SubTotal,
    decimal TaxAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceAmount,
    InvoiceStatus Status,
    DateTime? DueDate,
    DateTime? IssuedAt,
    DateTime? PaidAt,
    DateTime? VoidedAt,
    string? VoidReason,
    string? Currency,
    DateTime CreatedAt,
    List<InvoiceLineItemDto> LineItems,
    List<InvoicePaymentDto> Payments
);

public record InvoiceLineItemDto(
    Guid Id,
    string? Description,
    string ItemType,
    string? ItemReference,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    decimal? DiscountAmount,
    decimal? TaxAmount,
    decimal NetTotal
);

public record InvoicePaymentDto(
    Guid Id,
    decimal Amount,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    string? PaymentReference,
    DateTime PaidAt
);

public record CreateInvoiceRequest(
    Guid? AthleteId,
    Guid? AcademyId,
    string? Description,
    DateTime? DueDate,
    string? Currency,
    List<CreateInvoiceLineItemDto> LineItems,
    string? CouponCode,
    Guid? ScholarshipId
);

public record CreateInvoiceLineItemDto(
    string? Description,
    string ItemType,
    string? ItemReference,
    int Quantity,
    decimal UnitPrice,
    string? CouponCode
);

public record UpdateInvoiceRequest(
    string? Description,
    DateTime? DueDate,
    List<CreateInvoiceLineItemDto>? LineItems
);

public record InvoiceSummaryDto(
    Guid Id,
    string InvoiceNumber,
    string? AthleteName,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceAmount,
    InvoiceStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt
);

public record InvoiceSearchRequest(
    string? SearchTerm,
    InvoiceStatus? Status,
    Guid? AthleteId,
    Guid? AcademyId,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 20
);
