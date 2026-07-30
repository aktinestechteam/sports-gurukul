using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class CreditWalletCommandHandlerTests
{
    private readonly Mock<IWalletService> _serviceMock;
    private readonly CreditWalletCommandHandler _handler;

    public CreditWalletCommandHandlerTests()
    {
        _serviceMock = new Mock<IWalletService>();
        _handler = new CreditWalletCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var walletId = Guid.NewGuid();
        var command = new CreditWalletCommand(walletId, 500m, "REF-001", "Top up");
        var expected = Result<WalletDto>.Success(new WalletDto(walletId, Guid.NewGuid(), 500m, "INR", DateTime.UtcNow, DateTime.UtcNow));
        _serviceMock.Setup(s => s.CreditWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CreditWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CreditWalletCommand(Guid.NewGuid(), 500m, null, null);
        _serviceMock.Setup(s => s.CreditWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Credit failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Credit failed");
    }
}

public class DebitWalletCommandHandlerTests
{
    private readonly Mock<IWalletService> _serviceMock;
    private readonly DebitWalletCommandHandler _handler;

    public DebitWalletCommandHandlerTests()
    {
        _serviceMock = new Mock<IWalletService>();
        _handler = new DebitWalletCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var walletId = Guid.NewGuid();
        var command = new DebitWalletCommand(walletId, 200m, "REF-002", "Purchase");
        var expected = Result<WalletDto>.Success(new WalletDto(walletId, Guid.NewGuid(), 300m, "INR", DateTime.UtcNow, DateTime.UtcNow));
        _serviceMock.Setup(s => s.DebitWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.DebitWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new DebitWalletCommand(Guid.NewGuid(), 200m, null, null);
        _serviceMock.Setup(s => s.DebitWalletAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Debit failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Debit failed");
    }
}

public class TransferWalletBalanceCommandHandlerTests
{
    private readonly Mock<IWalletService> _serviceMock;
    private readonly TransferWalletBalanceCommandHandler _handler;

    public TransferWalletBalanceCommandHandlerTests()
    {
        _serviceMock = new Mock<IWalletService>();
        _handler = new TransferWalletBalanceCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var fromWalletId = Guid.NewGuid();
        var toWalletId = Guid.NewGuid();
        var command = new TransferWalletBalanceCommand(fromWalletId, toWalletId, 300m, "Transfer");
        var expected = Result<WalletDto>.Success(new WalletDto(fromWalletId, Guid.NewGuid(), 200m, "INR", DateTime.UtcNow, DateTime.UtcNow));
        _serviceMock.Setup(s => s.TransferBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.TransferBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new TransferWalletBalanceCommand(Guid.NewGuid(), Guid.NewGuid(), 300m, null);
        _serviceMock.Setup(s => s.TransferBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Transfer failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Transfer failed");
    }
}
