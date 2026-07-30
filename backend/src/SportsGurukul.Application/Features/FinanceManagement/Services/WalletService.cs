using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Features.FinanceManagement.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ILedgerService _ledgerService;

    public WalletService(IWalletRepository walletRepository, ILedgerService ledgerService)
    {
        _walletRepository = walletRepository;
        _ledgerService = ledgerService;
    }

    public async Task<Result<WalletDto>> CreateWalletAsync(Guid userId, string currency, CancellationToken cancellationToken)
    {
        var existing = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
            return Result<WalletDto>.Failure("User already has a wallet");

        var wallet = new Wallet
        {
            UserId = userId,
            Balance = 0,
            Currency = currency,
            IsActive = true,
        };

        var created = await _walletRepository.AddAsync(wallet, cancellationToken);
        return Result<WalletDto>.Success(MapToDto(created));
    }

    public async Task<Result<WalletDto>> CreditWalletAsync(Guid walletId, decimal amount, string? reference, string? description, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdWithTransactionsAsync(walletId, cancellationToken);
        if (wallet is null)
            return Result<WalletDto>.Failure("Wallet not found");

        var balanceBefore = wallet.Balance;
        wallet.Balance += amount;

        wallet.Transactions.Add(new WalletTransaction
        {
            TransactionType = TransactionType.Credit,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Reference = reference,
            Description = description,
        });

        _walletRepository.Update(wallet);

        await PostWalletCreditLedger(wallet, amount, reference, cancellationToken);

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    public async Task<Result<WalletDto>> DebitWalletAsync(Guid walletId, decimal amount, string? reference, string? description, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdWithTransactionsAsync(walletId, cancellationToken);
        if (wallet is null)
            return Result<WalletDto>.Failure("Wallet not found");

        if (wallet.Balance < amount)
            return Result<WalletDto>.Failure("Insufficient balance");

        var balanceBefore = wallet.Balance;
        wallet.Balance -= amount;

        wallet.Transactions.Add(new WalletTransaction
        {
            TransactionType = TransactionType.Debit,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Reference = reference,
            Description = description,
        });

        _walletRepository.Update(wallet);
        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    public async Task<Result<WalletDto>> TransferBalanceAsync(Guid fromWalletId, Guid toWalletId, decimal amount, string? description, CancellationToken cancellationToken)
    {
        var fromWallet = await _walletRepository.GetByIdWithTransactionsAsync(fromWalletId, cancellationToken);
        if (fromWallet is null)
            return Result<WalletDto>.Failure("Source wallet not found");

        var toWallet = await _walletRepository.GetByIdWithTransactionsAsync(toWalletId, cancellationToken);
        if (toWallet is null)
            return Result<WalletDto>.Failure("Destination wallet not found");

        if (fromWallet.Balance < amount)
            return Result<WalletDto>.Failure("Insufficient balance in source wallet");

        var fromBalanceBefore = fromWallet.Balance;
        fromWallet.Balance -= amount;
        fromWallet.Transactions.Add(new WalletTransaction
        {
            TransactionType = TransactionType.Debit,
            Amount = amount,
            BalanceBefore = fromBalanceBefore,
            BalanceAfter = fromWallet.Balance,
            Description = $"Transfer to {toWalletId}: {description}",
        });

        var toBalanceBefore = toWallet.Balance;
        toWallet.Balance += amount;
        toWallet.Transactions.Add(new WalletTransaction
        {
            TransactionType = TransactionType.Credit,
            Amount = amount,
            BalanceBefore = toBalanceBefore,
            BalanceAfter = toWallet.Balance,
            Description = $"Transfer from {fromWalletId}: {description}",
        });

        _walletRepository.Update(fromWallet);
        _walletRepository.Update(toWallet);

        return Result<WalletDto>.Success(MapToDto(fromWallet));
    }

    public async Task<Result<WalletDto>> GetBalanceAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
            return Result<WalletDto>.Failure("Wallet not found");

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    public async Task<Result<WalletDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
        if (wallet is null)
            return Result<WalletDto>.Failure("Wallet not found");

        return Result<WalletDto>.Success(MapToDto(wallet));
    }

    public async Task<Result<IReadOnlyList<WalletTransactionDto>>> GetTransactionsAsync(Guid walletId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdWithTransactionsAsync(walletId, cancellationToken);
        if (wallet is null)
            return Result<IReadOnlyList<WalletTransactionDto>>.Failure("Wallet not found");

        var transactions = wallet.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new WalletTransactionDto(
                t.Id, t.WalletId, t.TransactionType, t.Amount, t.BalanceBefore, t.BalanceAfter,
                t.Reference, t.Description, t.CreatedAt
            ))
            .ToList();

        return Result<IReadOnlyList<WalletTransactionDto>>.Success(transactions);
    }

    private async Task PostWalletCreditLedger(Wallet wallet, decimal amount, string? reference, CancellationToken cancellationToken)
    {
        var walletLiability = await _ledgerService.GetOrCreateLedgerAsync("WALL", "Wallet Liabilities", LedgerType.Liability, "Customer Wallet Balances", cancellationToken);
        if (walletLiability.IsSuccess)
        {
            await _ledgerService.PostLedgerEntryAsync(walletLiability.Value!, new LedgerEntry
            {
                DebitAmount = amount,
                CreditAmount = 0,
                Description = $"Wallet credit: {reference ?? wallet.Id.ToString()}",
                Reference = wallet.Id.ToString(),
                EntryDate = DateTime.UtcNow,
            }, cancellationToken);
        }
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto(
            wallet.Id,
            wallet.UserId,
            wallet.Balance,
            wallet.Currency,
            wallet.CreatedAt,
            wallet.Transactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault()?.CreatedAt
        );
    }
}
