using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record SettlementDto(
    Guid Id,
    string BatchNumber,
    decimal TotalAmount,
    int PaymentCount,
    Domain.Enums.Finance.SettlementStatus Status,
    string? GatewayReference,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    List<SettlementItemDto> Settlements
);

public record SettlementItemDto(
    Guid Id,
    Guid PaymentId,
    string PaymentReference,
    decimal Amount,
    Domain.Enums.Finance.SettlementStatus Status
);
