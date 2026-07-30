using SportsGurukul.Domain.Entities.Finance;

namespace SportsGurukul.Application.Common.Interfaces.Finance;

public interface IWalletRepository : IRepository<Wallet>
{
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WalletTransaction?> GetLastTransactionAsync(Guid walletId, CancellationToken cancellationToken = default);
}
