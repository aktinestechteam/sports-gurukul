using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class CreateWalletEdgeCaseTests
{
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly CreateWalletCommandHandler _handler;

    public CreateWalletEdgeCaseTests()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _handler = new CreateWalletCommandHandler(_walletServiceMock.Object);
    }

    [Fact]
    public async Task CreateWallet_UserAlreadyHasWallet_ShouldFail()
    {
        var userId = Guid.NewGuid();
        var command = new CreateWalletCommand(userId, "INR");

        _walletServiceMock.Setup(s => s.CreateWalletAsync(userId, "INR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("User already has a wallet"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User already has a wallet");
        _walletServiceMock.Verify(s => s.CreateWalletAsync(userId, "INR", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CreditWalletEdgeCaseTests
{
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly CreditWalletCommandHandler _handler;

    public CreditWalletEdgeCaseTests()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _handler = new CreditWalletCommandHandler(_walletServiceMock.Object);
    }

    [Fact]
    public async Task CreditWallet_NegativeAmount_ShouldFail()
    {
        var walletId = Guid.NewGuid();
        var command = new CreditWalletCommand(walletId, -100m, "REF001", "Negative credit");

        _walletServiceMock.Setup(s => s.CreditWalletAsync(walletId, -100m, "REF001", "Negative credit", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Credit amount must be positive"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credit amount must be positive");
        _walletServiceMock.Verify(s => s.CreditWalletAsync(walletId, -100m, "REF001", "Negative credit", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class DebitWalletEdgeCaseTests
{
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly DebitWalletCommandHandler _handler;

    public DebitWalletEdgeCaseTests()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _handler = new DebitWalletCommandHandler(_walletServiceMock.Object);
    }

    [Fact]
    public async Task DebitWallet_InsufficientBalance_ShouldFail()
    {
        var walletId = Guid.NewGuid();
        var command = new DebitWalletCommand(walletId, 5000m, "REF002", "Large debit");

        _walletServiceMock.Setup(s => s.DebitWalletAsync(walletId, 5000m, "REF002", "Large debit", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Insufficient wallet balance"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Insufficient wallet balance");
        _walletServiceMock.Verify(s => s.DebitWalletAsync(walletId, 5000m, "REF002", "Large debit", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class TransferWalletBalanceEdgeCaseTests
{
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly TransferWalletBalanceCommandHandler _handler;

    public TransferWalletBalanceEdgeCaseTests()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _handler = new TransferWalletBalanceCommandHandler(_walletServiceMock.Object);
    }

    [Fact]
    public async Task TransferBalance_SameWallet_ShouldFail()
    {
        var walletId = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(walletId, walletId, 100m, "Self transfer");

        _walletServiceMock.Setup(s => s.TransferBalanceAsync(walletId, walletId, 100m, "Self transfer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Cannot transfer to the same wallet"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot transfer to the same wallet");
        _walletServiceMock.Verify(s => s.TransferBalanceAsync(walletId, walletId, 100m, "Self transfer", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TransferBalance_InsufficientBalance_ShouldFail()
    {
        var fromWalletId = Guid.NewGuid();
        var toWalletId = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(fromWalletId, toWalletId, 9999m, "Large transfer");

        _walletServiceMock.Setup(s => s.TransferBalanceAsync(fromWalletId, toWalletId, 9999m, "Large transfer", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Insufficient balance in source wallet"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Insufficient balance in source wallet");
        _walletServiceMock.Verify(s => s.TransferBalanceAsync(fromWalletId, toWalletId, 9999m, "Large transfer", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TransferBalance_NonExistentDestination_ShouldFail()
    {
        var fromWalletId = Guid.NewGuid();
        var toWalletId = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(fromWalletId, toWalletId, 100m, "Transfer to unknown");

        _walletServiceMock.Setup(s => s.TransferBalanceAsync(fromWalletId, toWalletId, 100m, "Transfer to unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Destination wallet not found"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Destination wallet not found");
        _walletServiceMock.Verify(s => s.TransferBalanceAsync(fromWalletId, toWalletId, 100m, "Transfer to unknown", It.IsAny<CancellationToken>()), Times.Once);
    }
}
