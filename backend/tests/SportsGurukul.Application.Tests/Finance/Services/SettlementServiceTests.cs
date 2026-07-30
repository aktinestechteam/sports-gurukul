using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using SettlementStatus = SportsGurukul.Domain.Enums.Finance.SettlementStatus;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class SettlementServiceTests
{
    private readonly Mock<ISettlementRepository> _settlementRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly SettlementService _service;

    public SettlementServiceTests()
    {
        _settlementRepoMock = new Mock<ISettlementRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _service = new SettlementService(_settlementRepoMock.Object, _paymentRepoMock.Object);
    }

    #region CreateSettlementBatchAsync

    [Fact]
    public async Task CreateSettlementBatchAsync_ValidPayments_ReturnsBatch()
    {
        var paymentId1 = Guid.NewGuid();
        var paymentId2 = Guid.NewGuid();

        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment { Id = paymentId1, Amount = 500m });
        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment { Id = paymentId2, Amount = 1500m });
        _settlementRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<SettlementBatch, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _settlementRepoMock.Setup(r => r.AddAsync(It.IsAny<SettlementBatch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch b, CancellationToken _) => b);

        var result = await _service.CreateSettlementBatchAsync(new[] { paymentId1, paymentId2 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(2000m);
        result.Value.PaymentCount.Should().Be(2);
        result.Value.Status.Should().Be(SettlementStatus.Pending);
        _settlementRepoMock.Verify(r => r.AddAsync(It.IsAny<SettlementBatch>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSettlementBatchAsync_PaymentNotFound_ReturnsFailure()
    {
        var paymentId = Guid.NewGuid();
        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.CreateSettlementBatchAsync(new[] { paymentId }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Payment not found: {paymentId}");
    }

    [Fact]
    public async Task CreateSettlementBatchAsync_EmptyPaymentIds_ReturnsEmptyBatch()
    {
        _settlementRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<SettlementBatch, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _settlementRepoMock.Setup(r => r.AddAsync(It.IsAny<SettlementBatch>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch b, CancellationToken _) => b);

        var result = await _service.CreateSettlementBatchAsync(Array.Empty<Guid>(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(0m);
        result.Value.PaymentCount.Should().Be(0);
    }

    #endregion

    #region ApproveSettlementAsync

    [Fact]
    public async Task ApproveSettlementAsync_PendingBatch_ReturnsInProgress()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch
        {
            Id = batchId,
            Status = SettlementStatus.Pending,
            Settlements = new List<Settlement>
            {
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.Pending }
            }
        };

        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.ApproveSettlementAsync(batchId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(SettlementStatus.InProgress);
        _settlementRepoMock.Verify(r => r.Update(batch), Times.Once);
    }

    [Fact]
    public async Task ApproveSettlementAsync_BatchNotFound_ReturnsFailure()
    {
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch?)null);

        var result = await _service.ApproveSettlementAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Settlement batch not found");
    }

    [Fact]
    public async Task ApproveSettlementAsync_NotPending_ReturnsFailure()
    {
        var batch = new SettlementBatch { Id = Guid.NewGuid(), Status = SettlementStatus.Completed };
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.ApproveSettlementAsync(batch.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only pending batches can be approved");
    }

    [Fact]
    public async Task ApproveSettlementAsync_UpdatesAllSettlementsStatus()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch
        {
            Id = batchId,
            Status = SettlementStatus.Pending,
            Settlements = new List<Settlement>
            {
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.Pending },
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.Pending }
            }
        };

        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        await _service.ApproveSettlementAsync(batchId, CancellationToken.None);

        batch.Settlements.All(s => s.Status == SettlementStatus.InProgress).Should().BeTrue();
    }

    #endregion

    #region CompleteSettlementAsync

    [Fact]
    public async Task CompleteSettlementAsync_InProgressBatch_ReturnsCompleted()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch
        {
            Id = batchId,
            Status = SettlementStatus.InProgress,
            Settlements = new List<Settlement>
            {
                new() { Id = Guid.NewGuid(), PaymentId = Guid.NewGuid(), Amount = 500m, Status = SettlementStatus.InProgress }
            }
        };

        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.CompleteSettlementAsync(batchId, "BANK-REF-001", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(SettlementStatus.Completed);
        _settlementRepoMock.Verify(r => r.Update(batch), Times.Once);
    }

    [Fact]
    public async Task CompleteSettlementAsync_BatchNotFound_ReturnsFailure()
    {
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch?)null);

        var result = await _service.CompleteSettlementAsync(Guid.NewGuid(), "REF", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Settlement batch not found");
    }

    [Fact]
    public async Task CompleteSettlementAsync_NotInProgress_ReturnsFailure()
    {
        var batch = new SettlementBatch { Id = Guid.NewGuid(), Status = SettlementStatus.Pending };
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.CompleteSettlementAsync(batch.Id, "REF", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only in-progress batches can be completed");
    }

    [Fact]
    public async Task CompleteSettlementAsync_WithoutReference_StillSucceeds()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch
        {
            Id = batchId,
            Status = SettlementStatus.InProgress,
            Settlements = new List<Settlement>
            {
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.InProgress }
            }
        };

        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.CompleteSettlementAsync(batchId, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(SettlementStatus.Completed);
    }

    [Fact]
    public async Task CompleteSettlementAsync_UpdatesAllChildSettlements()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch
        {
            Id = batchId,
            Status = SettlementStatus.InProgress,
            Settlements = new List<Settlement>
            {
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.InProgress },
                new() { Id = Guid.NewGuid(), Status = SettlementStatus.InProgress }
            }
        };

        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        await _service.CompleteSettlementAsync(batchId, "REF", CancellationToken.None);

        batch.Settlements.All(s => s.Status == SettlementStatus.Completed).Should().BeTrue();
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Exists_ReturnsBatch()
    {
        var batchId = Guid.NewGuid();
        var batch = new SettlementBatch { Id = batchId, BatchNumber = "STL-001", Status = SettlementStatus.Pending };
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _service.GetByIdAsync(batchId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(batchId);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _settlementRepoMock.Setup(r => r.GetByIdWithSettlementsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementBatch?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Settlement batch not found");
    }

    #endregion

    #region GenerateBatchNumberAsync

    [Fact]
    public async Task GenerateBatchNumberAsync_ReturnsFormattedNumber()
    {
        _settlementRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<SettlementBatch, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(12);

        var result = await _service.GenerateBatchNumberAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Match("STL-20260730-00013");
    }

    #endregion
}
