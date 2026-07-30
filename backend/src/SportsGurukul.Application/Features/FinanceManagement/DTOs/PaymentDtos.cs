using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    string PaymentReference,
    decimal Amount,
    decimal? TaxAmount,
    decimal? GatewayFee,
    decimal NetAmount,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    string? IdempotencyKey,
    string? GatewayReference,
    string? FailureReason,
    DateTime? PaidAt,
    DateTime? RefundedAt,
    DateTime CreatedAt
);

public record InitiatePaymentRequest(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? IdempotencyKey,
    string? Description
);

public record RecordOfflinePaymentRequest(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string? Reference,
    DateTime PaidAt,
    string? Description
);
