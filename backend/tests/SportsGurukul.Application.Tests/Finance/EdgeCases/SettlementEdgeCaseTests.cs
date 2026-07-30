using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Settlement;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class CreateSettlementBatchEdgeCaseTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly CreateSettlementBatchCommandHandler _handler;

    public CreateSettlementBatchEdgeCaseTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _handler = new CreateSettlementBatchCommandHandler(_settlementServiceMock.Object);
    }

    [Fact]
    public async Task CreateSettlementBatch_EmptyPaymentIds_ShouldFail()
    {
        var command = new CreateSettlementBatchCommand(Array.Empty<Guid>());

        _settlementServiceMock.Setup(s => s.CreateSettlementBatchAsync(It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("At least one payment is required to create a batch"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("At least one payment is required to create a batch");
        _settlementServiceMock.Verify(s => s.CreateSettlementBatchAsync(It.Is<Guid[]>(ids => ids.Length == 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSettlementBatch_NonExistentPayment_ShouldFail()
    {
        var paymentIds = new[] { Guid.NewGuid() };
        var command = new CreateSettlementBatchCommand(paymentIds);

        _settlementServiceMock.Setup(s => s.CreateSettlementBatchAsync(paymentIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("One or more payments not found"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("One or more payments not found");
        _settlementServiceMock.Verify(s => s.CreateSettlementBatchAsync(paymentIds, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ApproveSettlementEdgeCaseTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly ApproveSettlementCommandHandler _handler;

    public ApproveSettlementEdgeCaseTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _handler = new ApproveSettlementCommandHandler(_settlementServiceMock.Object);
    }

    [Fact]
    public async Task ApproveSettlement_NonPendingBatch_ShouldFail()
    {
        var batchId = Guid.NewGuid();
        var command = new ApproveSettlementCommand(batchId);

        _settlementServiceMock.Setup(s => s.ApproveSettlementAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Only pending settlement batches can be approved"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only pending settlement batches can be approved");
        _settlementServiceMock.Verify(s => s.ApproveSettlementAsync(batchId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CompleteSettlementEdgeCaseTests
{
    private readonly Mock<ISettlementService> _settlementServiceMock;
    private readonly CompleteSettlementCommandHandler _handler;

    public CompleteSettlementEdgeCaseTests()
    {
        _settlementServiceMock = new Mock<ISettlementService>();
        _handler = new CompleteSettlementCommandHandler(_settlementServiceMock.Object);
    }

    [Fact]
    public async Task CompleteSettlement_NotInProgress_ShouldFail()
    {
        var batchId = Guid.NewGuid();
        var command = new CompleteSettlementCommand(batchId, "BANK-REF-001");

        _settlementServiceMock.Setup(s => s.CompleteSettlementAsync(batchId, "BANK-REF-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Only in-progress settlement batches can be completed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only in-progress settlement batches can be completed");
        _settlementServiceMock.Verify(s => s.CompleteSettlementAsync(batchId, "BANK-REF-001", It.IsAny<CancellationToken>()), Times.Once);
    }
}
