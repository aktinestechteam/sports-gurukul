using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.DTOs;

public record WalletDto(
    Guid Id,
    Guid UserId,
    decimal Balance,
    string Currency,
    DateTime CreatedAt,
    DateTime? LastTransactionAt
);

public record WalletTransactionDto(
    Guid Id,
    Guid WalletId,
    TransactionType Type,
    decimal Amount,
    decimal BalanceBefore,
    decimal BalanceAfter,
    string? Reference,
    string? Description,
    DateTime CreatedAt
);
