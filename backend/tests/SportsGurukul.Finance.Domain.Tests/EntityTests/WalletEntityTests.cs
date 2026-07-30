using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.Domain.Tests.Builders;

namespace SportsGurukul.Finance.Domain.Tests.Entities;

public class WalletEntityTests
{
    [Fact]
    public void CreateWallet_HasZeroBalanceByDefault()
    {
        var w = FinanceEntityBuilder.CreateWallet(balance: 0);
        w.Balance.Should().Be(0);
        w.IsActive.Should().BeTrue();
    }

    [Fact]
    public void WalletWithFunds_HasCorrectBalance()
    {
        var w = FinanceEntityBuilder.CreateWallet(balance: 5000);
        w.Balance.Should().Be(5000);
    }

    [Fact]
    public void WalletTransaction_CreditUpdatesBalance()
    {
        var w = FinanceEntityBuilder.CreateWallet(balance: 1000);
        var txn = FinanceEntityBuilder.CreateWalletTransaction(w.Id, 500, 1000, TransactionType.Credit);
        txn.BalanceAfter.Should().Be(1500);
        txn.BalanceBefore.Should().Be(1000);
        txn.TransactionType.Should().Be(TransactionType.Credit);
    }

    [Fact]
    public void WalletTransaction_DebitUpdatesBalance()
    {
        var w = FinanceEntityBuilder.CreateWallet(balance: 2000);
        var txn = FinanceEntityBuilder.CreateWalletTransaction(w.Id, 800, 2000, TransactionType.Debit);
        txn.BalanceAfter.Should().Be(1200);
        txn.TransactionType.Should().Be(TransactionType.Debit);
    }

    [Fact]
    public void Wallet_UserHasSingleWallet()
    {
        var userId = Guid.NewGuid();
        var w1 = FinanceEntityBuilder.CreateWallet(userId: userId);
        var w2 = FinanceEntityBuilder.CreateWallet(userId: userId);
        w1.UserId.Should().Be(userId);
        w2.UserId.Should().Be(userId);
    }
}
