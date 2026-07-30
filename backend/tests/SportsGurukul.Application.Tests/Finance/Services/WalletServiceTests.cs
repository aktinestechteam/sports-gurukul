using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class WalletServiceTests
{
    private readonly Mock<IWalletRepository> _walletRepoMock;
    private readonly Mock<ILedgerService> _ledgerServiceMock;
    private readonly WalletService _service;

    public WalletServiceTests()
    {
        _walletRepoMock = new Mock<IWalletRepository>();
        _ledgerServiceMock = new Mock<ILedgerService>();
        _service = new WalletService(_walletRepoMock.Object, _ledgerServiceMock.Object);
    }

    #region CreateWalletAsync

    [Fact]
    public async Task CreateWalletAsync_NewUser_ReturnsWallet()
    {
        var userId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);
        _walletRepoMock.Setup(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet w, CancellationToken _) => w);

        var result = await _service.CreateWalletAsync(userId, "INR", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(0);
        result.Value.Currency.Should().Be("INR");
        _walletRepoMock.Verify(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateWalletAsync_ExistingUser_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { UserId = userId });

        var result = await _service.CreateWalletAsync(userId, "INR", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User already has a wallet");
    }

    [Fact]
    public async Task CreateWalletAsync_DifferentCurrency_UsesProvidedCurrency()
    {
        var userId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        Wallet? captured = null;
        _walletRepoMock.Setup(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()))
            .Callback((Wallet w, CancellationToken _) => captured = w)
            .ReturnsAsync((Wallet w, CancellationToken _) => w);

        await _service.CreateWalletAsync(userId, "USD", CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Currency.Should().Be("USD");
    }

    #endregion

    #region CreditWalletAsync

    [Fact]
    public async Task CreditWalletAsync_ValidWallet_ReturnsUpdatedBalance()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Balance = 1000m,
            Currency = "INR",
            IsActive = true,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var liabilityLedger = new Ledger { Id = Guid.NewGuid(), Code = "WALL" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("WALL", "Wallet Liabilities", LedgerType.Liability, "Customer Wallet Balances", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(liabilityLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CreditWalletAsync(walletId, 500m, "REF-001", "Top up", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(1500m);
        _walletRepoMock.Verify(r => r.Update(wallet), Times.Once);
    }

    [Fact]
    public async Task CreditWalletAsync_WalletNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.CreditWalletAsync(Guid.NewGuid(), 100m, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    [Fact]
    public async Task CreditWalletAsync_AddsTransactionRecord()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Balance = 1000m,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var liabilityLedger = new Ledger { Id = Guid.NewGuid(), Code = "WALL" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("WALL", "Wallet Liabilities", LedgerType.Liability, "Customer Wallet Balances", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(liabilityLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        await _service.CreditWalletAsync(walletId, 300m, "REF-002", "Bonus", CancellationToken.None);

        wallet.Transactions.Should().HaveCount(1);
        var txn = wallet.Transactions.First();
        txn.TransactionType.Should().Be(TransactionType.Credit);
        txn.Amount.Should().Be(300m);
        txn.BalanceBefore.Should().Be(1000m);
        txn.BalanceAfter.Should().Be(1300m);
    }

    #endregion

    #region DebitWalletAsync

    [Fact]
    public async Task DebitWalletAsync_SufficientBalance_ReturnsUpdatedWallet()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Balance = 2000m,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.DebitWalletAsync(walletId, 500m, "REF-DEBIT", "Purchase", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(1500m);
        _walletRepoMock.Verify(r => r.Update(wallet), Times.Once);
    }

    [Fact]
    public async Task DebitWalletAsync_InsufficientBalance_ReturnsFailure()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet { Id = walletId, Balance = 100m };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.DebitWalletAsync(walletId, 500m, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Insufficient balance");
    }

    [Fact]
    public async Task DebitWalletAsync_WalletNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.DebitWalletAsync(Guid.NewGuid(), 100m, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    [Fact]
    public async Task DebitWalletAsync_ExactBalance_ReturnsEmptyBalance()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Balance = 500m,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.DebitWalletAsync(walletId, 500m, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(0);
    }

    [Fact]
    public async Task DebitWalletAsync_RecordsDebitTransaction()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Balance = 1000m,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        await _service.DebitWalletAsync(walletId, 400m, "REF-3", "Withdraw", CancellationToken.None);

        wallet.Transactions.Should().HaveCount(1);
        var txn = wallet.Transactions.First();
        txn.TransactionType.Should().Be(TransactionType.Debit);
        txn.Amount.Should().Be(400m);
        txn.BalanceBefore.Should().Be(1000m);
        txn.BalanceAfter.Should().Be(600m);
    }

    #endregion

    #region TransferBalanceAsync

    [Fact]
    public async Task TransferBalanceAsync_ValidTransfer_ReturnsUpdatedFromWallet()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();
        var fromWallet = new Wallet
        {
            Id = fromId,
            Balance = 1000m,
            Transactions = new List<WalletTransaction>()
        };
        var toWallet = new Wallet
        {
            Id = toId,
            Balance = 500m,
            Transactions = new List<WalletTransaction>()
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(fromId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromWallet);
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(toId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(toWallet);

        var result = await _service.TransferBalanceAsync(fromId, toId, 300m, "Gift", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        fromWallet.Balance.Should().Be(700m);
        toWallet.Balance.Should().Be(800m);
        _walletRepoMock.Verify(r => r.Update(fromWallet), Times.Once);
        _walletRepoMock.Verify(r => r.Update(toWallet), Times.Once);
    }

    [Fact]
    public async Task TransferBalanceAsync_SourceNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.TransferBalanceAsync(Guid.NewGuid(), Guid.NewGuid(), 100m, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Source wallet not found");
    }

    [Fact]
    public async Task TransferBalanceAsync_DestinationNotFound_ReturnsFailure()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.Is<Guid>(g => g == fromId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { Id = fromId, Balance = 1000m, Transactions = new List<WalletTransaction>() });
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.Is<Guid>(g => g == toId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.TransferBalanceAsync(fromId, toId, 100m, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Destination wallet not found");
    }

    [Fact]
    public async Task TransferBalanceAsync_InsufficientBalance_ReturnsFailure()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(fromId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { Id = fromId, Balance = 50m });
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(toId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { Id = toId, Balance = 100m });

        var result = await _service.TransferBalanceAsync(fromId, toId, 100m, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Insufficient balance in source wallet");
    }

    [Fact]
    public async Task TransferBalanceAsync_RecordsTransactionsOnBothWallets()
    {
        var fromId = Guid.NewGuid();
        var toId = Guid.NewGuid();
        var fromWallet = new Wallet { Id = fromId, Balance = 500m, Transactions = new List<WalletTransaction>() };
        var toWallet = new Wallet { Id = toId, Balance = 200m, Transactions = new List<WalletTransaction>() };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(fromId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromWallet);
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(toId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(toWallet);

        await _service.TransferBalanceAsync(fromId, toId, 200m, "Transfer", CancellationToken.None);

        fromWallet.Transactions.Should().HaveCount(1);
        fromWallet.Transactions.First().TransactionType.Should().Be(TransactionType.Debit);
        fromWallet.Transactions.First().Amount.Should().Be(200m);

        toWallet.Transactions.Should().HaveCount(1);
        toWallet.Transactions.First().TransactionType.Should().Be(TransactionType.Credit);
        toWallet.Transactions.First().Amount.Should().Be(200m);
    }

    #endregion

    #region GetBalanceAsync

    [Fact]
    public async Task GetBalanceAsync_WalletExists_ReturnsWallet()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet { Id = walletId, Balance = 5000m, Currency = "INR", UserId = Guid.NewGuid() };
        _walletRepoMock.Setup(r => r.GetByIdAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.GetBalanceAsync(walletId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Balance.Should().Be(5000m);
    }

    [Fact]
    public async Task GetBalanceAsync_WalletNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.GetBalanceAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    #endregion

    #region GetByUserIdAsync

    [Fact]
    public async Task GetByUserIdAsync_WalletExists_ReturnsWallet()
    {
        var userId = Guid.NewGuid();
        var wallet = new Wallet { Id = Guid.NewGuid(), UserId = userId, Balance = 3000m };
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.GetByUserIdAsync(userId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByUserIdAsync_WalletNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.GetByUserIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    #endregion

    #region GetTransactionsAsync

    [Fact]
    public async Task GetTransactionsAsync_ValidWallet_ReturnsPagedTransactions()
    {
        var walletId = Guid.NewGuid();
        var wallet = new Wallet
        {
            Id = walletId,
            Transactions = new List<WalletTransaction>
            {
                new() { Id = Guid.NewGuid(), WalletId = walletId, TransactionType = TransactionType.Credit, Amount = 500, BalanceBefore = 0, BalanceAfter = 500, CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
                new() { Id = Guid.NewGuid(), WalletId = walletId, TransactionType = TransactionType.Debit, Amount = 100, BalanceBefore = 500, BalanceAfter = 400, CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
                new() { Id = Guid.NewGuid(), WalletId = walletId, TransactionType = TransactionType.Credit, Amount = 200, BalanceBefore = 400, BalanceAfter = 600, CreatedAt = DateTime.UtcNow }
            }
        };

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var result = await _service.GetTransactionsAsync(walletId, 1, 2, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Amount.Should().Be(200);
        result.Value[1].Amount.Should().Be(100);
    }

    [Fact]
    public async Task GetTransactionsAsync_WalletNotFound_ReturnsFailure()
    {
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var result = await _service.GetTransactionsAsync(Guid.NewGuid(), 1, 20, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    [Fact]
    public async Task GetTransactionsAsync_SecondPage_ReturnsCorrectPage()
    {
        var walletId = Guid.NewGuid();
        var transactions = Enumerable.Range(1, 5).Select(i => new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            TransactionType = TransactionType.Credit,
            Amount = i * 100m,
            BalanceBefore = (i - 1) * 100m,
            BalanceAfter = i * 100m,
            CreatedAt = DateTime.UtcNow.AddMinutes(-i)
        }).ToList();

        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { Id = walletId, Transactions = transactions });

        var result = await _service.GetTransactionsAsync(walletId, 2, 2, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Amount.Should().Be(300m);
        result.Value[1].Amount.Should().Be(400m);
    }

    [Fact]
    public async Task GetTransactionsAsync_EmptyTransactions_ReturnsEmptyList()
    {
        var walletId = Guid.NewGuid();
        _walletRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(walletId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Wallet { Id = walletId, Transactions = new List<WalletTransaction>() });

        var result = await _service.GetTransactionsAsync(walletId, 1, 20, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion
}
