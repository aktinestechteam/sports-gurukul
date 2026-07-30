using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Finance.Services;

public interface IWalletService
{
    Task<Result<WalletDto>> CreateWalletAsync(Guid userId, string currency = "INR", CancellationToken cancellationToken = default);
    Task<Result<WalletDto>> CreditWalletAsync(Guid walletId, decimal amount, string? reference, string? description, CancellationToken cancellationToken = default);
    Task<Result<WalletDto>> DebitWalletAsync(Guid walletId, decimal amount, string? reference, string? description, CancellationToken cancellationToken = default);
    Task<Result<WalletDto>> TransferBalanceAsync(Guid fromWalletId, Guid toWalletId, decimal amount, string? description, CancellationToken cancellationToken = default);
    Task<Result<WalletDto>> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken = default);
    Task<Result<WalletDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<WalletTransactionDto>>> GetTransactionsAsync(Guid walletId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
