using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record RefundDto(
    Guid Id,
    Guid PaymentId,
    string RefundNumber,
    decimal Amount,
    string? Reason,
    RefundStatus Status,
    string? ApprovedBy,
    string? RejectionReason,
    string? GatewayReference,
    DateTime? CompletedAt,
    DateTime CreatedAt
);

public record RequestRefundRequest(
    Guid PaymentId,
    decimal Amount,
    string? Reason,
    List<RefundItemRequest>? Items
);

public record RefundItemRequest(
    string? Description,
    decimal Amount
);
