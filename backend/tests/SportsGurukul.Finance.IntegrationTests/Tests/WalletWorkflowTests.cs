using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using SportsGurukul.Infrastructure.Persistence;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class WalletWorkflowTests : FinanceTestBase
{
    public WalletWorkflowTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<WalletDto> CreateWalletAsync(Guid userId)
    {
        var command = new CreateWalletCommand(userId, "INR");
        var result = await SendAsync<Result<WalletDto>>(command);
        return result.Value!;
    }

    [Fact]
    public async Task CreateWallet_ForNewUser_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();

        var result = await SendAsync<Result<WalletDto>>(
            new CreateWalletCommand(userId, "INR"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Balance.Should().Be(0);
        result.Value.Currency.Should().Be("INR");
    }

    [Fact]
    public async Task CreateWallet_ForExistingWallet_ReturnsFailure()
    {
        var result = await SendAsync<Result<WalletDto>>(
            new CreateWalletCommand(FinanceTestIds.AthleteUserId, "INR"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreditWallet_IncreasesBalance()
    {
        var wallet = await CreateWalletAsync(Guid.NewGuid());

        var creditResult = await SendAsync<Result<WalletDto>>(
            new CreditWalletCommand(wallet.Id, 500m, "REF-001", "Test credit"));

        creditResult.IsSuccess.Should().BeTrue();
        creditResult.Value!.Balance.Should().Be(500m);
    }

    [Fact]
    public async Task DebitWallet_WithSufficientBalance_DecreasesBalance()
    {
        var wallet = await CreateWalletAsync(Guid.NewGuid());
        await SendAsync<Result<WalletDto>>(new CreditWalletCommand(wallet.Id, 1000m, null, null));

        var debitResult = await SendAsync<Result<WalletDto>>(
            new DebitWalletCommand(wallet.Id, 300m, "REF-002", "Test debit"));

        debitResult.IsSuccess.Should().BeTrue();
        debitResult.Value!.Balance.Should().Be(700m);
    }

    [Fact]
    public async Task DebitWallet_WithInsufficientBalance_ReturnsFailure()
    {
        var wallet = await CreateWalletAsync(Guid.NewGuid());

        var debitResult = await SendAsync<Result<WalletDto>>(
            new DebitWalletCommand(wallet.Id, 100m, null, null));

        debitResult.IsSuccess.Should().BeFalse();
        debitResult.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task TransferWallet_ToAnotherUser_Succeeds()
    {
        var sourceWallet = await CreateWalletAsync(Guid.NewGuid());
        var destWallet = await CreateWalletAsync(Guid.NewGuid());
        await SendAsync<Result<WalletDto>>(new CreditWalletCommand(sourceWallet.Id, 1000m, null, null));

        var transferResult = await SendAsync<Result<WalletDto>>(
            new TransferWalletBalanceCommand(sourceWallet.Id, destWallet.Id, 400m, "Test transfer"));

        transferResult.IsSuccess.Should().BeTrue();
        transferResult.Value!.Balance.Should().Be(600m);
    }

    [Fact]
    public async Task TransferWallet_InsufficientBalance_ReturnsFailure()
    {
        var sourceWallet = await CreateWalletAsync(Guid.NewGuid());
        var destWallet = await CreateWalletAsync(Guid.NewGuid());

        var transferResult = await SendAsync<Result<WalletDto>>(
            new TransferWalletBalanceCommand(sourceWallet.Id, destWallet.Id, 100m, null));

        transferResult.IsSuccess.Should().BeFalse();
        transferResult.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetWalletByUserId_ReturnsWallet()
    {
        var userId = Guid.NewGuid();
        var wallet = await CreateWalletAsync(userId);

        var queryResult = await SendAsync<Result<WalletDto>>(
            new GetWalletByUserIdQuery(userId));

        queryResult.IsSuccess.Should().BeTrue();
        queryResult.Value!.Id.Should().Be(wallet.Id);
        queryResult.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetWalletTransactions_ReturnsHistory()
    {
        var wallet = await CreateWalletAsync(Guid.NewGuid());
        await SendAsync<Result<WalletDto>>(new CreditWalletCommand(wallet.Id, 500m, "REF-TXN", "Credit"));
        await SendAsync<Result<WalletDto>>(new DebitWalletCommand(wallet.Id, 200m, "REF-TXN2", "Debit"));

        var transactionsResult = await SendAsync<Result<IReadOnlyList<WalletTransactionDto>>>(
            new GetWalletTransactionsQuery(wallet.Id));

        transactionsResult.IsSuccess.Should().BeTrue();
        transactionsResult.Value.Should().HaveCount(2);
        transactionsResult.Value.Should().Contain(t => t.Type == TransactionType.Credit);
        transactionsResult.Value.Should().Contain(t => t.Type == TransactionType.Debit);
    }

    [Fact]
    public async Task WalletTransaction_Ledger_Consistency()
    {
        var wallet = await CreateWalletAsync(Guid.NewGuid());

        await SendAsync<Result<WalletDto>>(
            new CreditWalletCommand(wallet.Id, 1000m, "REF-LEDGER", "Credit with ledger"));

        var dbContext = ServiceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var ledgerEntries = await dbContext.LedgerEntries
            .Where(le => le.Reference == wallet.Id.ToString())
            .ToListAsync();

        ledgerEntries.Should().NotBeEmpty();
        ledgerEntries.Should().Contain(le => le.DebitAmount == 1000m);
    }
}
