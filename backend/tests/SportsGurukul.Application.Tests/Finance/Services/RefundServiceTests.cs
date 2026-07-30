using System.Linq.Expressions;
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

public class RefundServiceTests
{
    private readonly Mock<IRefundRepository> _refundRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<ILedgerService> _ledgerServiceMock;
    private readonly RefundService _service;

    public RefundServiceTests()
    {
        _refundRepoMock = new Mock<IRefundRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _ledgerServiceMock = new Mock<ILedgerService>();
        _service = new RefundService(
            _refundRepoMock.Object,
            _paymentRepoMock.Object,
            _ledgerServiceMock.Object);
    }

    #region RequestRefundAsync

    [Fact]
    public async Task RequestRefundAsync_ValidRequest_ReturnsRequestedRefund()
    {
        var paymentId = Guid.NewGuid();
        var request = new RequestRefundRequest(paymentId, 500m, "Damaged item", null);

        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment { Id = paymentId, Amount = 1000m, Status = PaymentStatus.Captured });
        _refundRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Refund, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _refundRepoMock.Setup(r => r.AddAsync(It.IsAny<Refund>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund r, CancellationToken _) => r);

        var result = await _service.RequestRefundAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(RefundStatus.Requested);
        result.Value.Amount.Should().Be(500m);
        _refundRepoMock.Verify(r => r.AddAsync(It.IsAny<Refund>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestRefundAsync_WithItems_AddsRefundItems()
    {
        var paymentId = Guid.NewGuid();
        var request = new RequestRefundRequest(paymentId, 300m, "Partial refund",
            new List<RefundItemRequest>
            {
                new("Item 1", 200m),
                new("Item 2", 100m)
            });

        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment { Id = paymentId, Amount = 500m, Status = PaymentStatus.Captured });
        _refundRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Refund, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _refundRepoMock.Setup(r => r.AddAsync(It.IsAny<Refund>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund r, CancellationToken _) => r);

        var result = await _service.RequestRefundAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _refundRepoMock.Verify(r => r.AddAsync(
            It.Is<Refund>(refund => refund.RefundItems.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestRefundAsync_PaymentNotFound_ReturnsFailure()
    {
        var request = new RequestRefundRequest(Guid.NewGuid(), 100m, "Reason", null);
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.RequestRefundAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
    }

    [Fact]
    public async Task RequestRefundAsync_NotCapturedPayment_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Amount = 1000m, Status = PaymentStatus.Pending };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var request = new RequestRefundRequest(payment.Id, 100m, "Reason", null);
        var result = await _service.RequestRefundAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Can only refund completed payments");
    }

    [Fact]
    public async Task RequestRefundAsync_AmountExceedsPayment_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Amount = 500m, Status = PaymentStatus.Captured };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var request = new RequestRefundRequest(payment.Id, 600m, "Reason", null);
        var result = await _service.RequestRefundAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund amount cannot exceed payment amount");
    }

    #endregion

    #region ApproveRefundAsync

    [Fact]
    public async Task ApproveRefundAsync_ValidRefund_ReturnsApproved()
    {
        var refundId = Guid.NewGuid();
        var refund = new Refund
        {
            Id = refundId,
            Status = RefundStatus.Requested,
            TotalAmount = 500m,
            RefundNumber = "RFN-001"
        };

        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(refundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var result = await _service.ApproveRefundAsync(refundId, "AdminUser", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(RefundStatus.Approved);
        _refundRepoMock.Verify(r => r.Update(refund), Times.Once);
    }

    [Fact]
    public async Task ApproveRefundAsync_RefundNotFound_ReturnsFailure()
    {
        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund?)null);

        var result = await _service.ApproveRefundAsync(Guid.NewGuid(), "Admin", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund not found");
    }

    [Fact]
    public async Task ApproveRefundAsync_NotRequested_ReturnsFailure()
    {
        var refund = new Refund { Id = Guid.NewGuid(), Status = RefundStatus.Approved };
        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var result = await _service.ApproveRefundAsync(refund.Id, "Admin", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only requested refunds can be approved");
    }

    #endregion

    #region RejectRefundAsync

    [Fact]
    public async Task RejectRefundAsync_ValidRefund_ReturnsRejected()
    {
        var refundId = Guid.NewGuid();
        var refund = new Refund { Id = refundId, Status = RefundStatus.Requested };

        _refundRepoMock.Setup(r => r.GetByIdAsync(refundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var result = await _service.RejectRefundAsync(refundId, "Not eligible", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(RefundStatus.Rejected);
        _refundRepoMock.Verify(r => r.Update(refund), Times.Once);
    }

    [Fact]
    public async Task RejectRefundAsync_RefundNotFound_ReturnsFailure()
    {
        _refundRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund?)null);

        var result = await _service.RejectRefundAsync(Guid.NewGuid(), "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund not found");
    }

    [Fact]
    public async Task RejectRefundAsync_NotRequested_ReturnsFailure()
    {
        var refund = new Refund { Id = Guid.NewGuid(), Status = RefundStatus.Completed };
        _refundRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var result = await _service.RejectRefundAsync(refund.Id, "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only requested refunds can be rejected");
    }

    #endregion

    #region CompleteRefundAsync

    [Fact]
    public async Task CompleteRefundAsync_ApprovedRefund_ReturnsCompleted()
    {
        var refundId = Guid.NewGuid();
        var refund = new Refund
        {
            Id = refundId,
            Status = RefundStatus.Approved,
            TotalAmount = 500m,
            RefundNumber = "RFN-001",
            PaymentId = Guid.NewGuid()
        };

        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(refundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var cashLedger = new Ledger { Id = Guid.NewGuid(), Code = "CASH" };
        var refundLedger = new Ledger { Id = Guid.NewGuid(), Code = "REF" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("CASH", "Cash", LedgerType.Asset, "Cash & Bank", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(cashLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("REF", "Refund Payable", LedgerType.Liability, "Refunds", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(refundLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CompleteRefundAsync(refundId, "GW-REF-001", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(RefundStatus.Completed);
        _refundRepoMock.Verify(r => r.Update(refund), Times.Once);
    }

    [Fact]
    public async Task CompleteRefundAsync_RefundNotFound_ReturnsFailure()
    {
        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Refund?)null);

        var result = await _service.CompleteRefundAsync(Guid.NewGuid(), "REF", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund not found");
    }

    [Fact]
    public async Task CompleteRefundAsync_NotApproved_ReturnsFailure()
    {
        var refund = new Refund { Id = Guid.NewGuid(), Status = RefundStatus.Requested };
        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var result = await _service.CompleteRefundAsync(refund.Id, "REF", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only approved refunds can be completed");
    }

    [Fact]
    public async Task CompleteRefundAsync_NullGatewayReference_StillSucceeds()
    {
        var refundId = Guid.NewGuid();
        var refund = new Refund
        {
            Id = refundId,
            Status = RefundStatus.Approved,
            TotalAmount = 200m,
            RefundNumber = "RFN-002",
            PaymentId = Guid.NewGuid()
        };

        _refundRepoMock.Setup(r => r.GetByIdWithItemsAsync(refundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refund);

        var cashLedger = new Ledger { Id = Guid.NewGuid(), Code = "CASH" };
        var refundLedger = new Ledger { Id = Guid.NewGuid(), Code = "REF" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("CASH", "Cash", LedgerType.Asset, "Cash & Bank", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(cashLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("REF", "Refund Payable", LedgerType.Liability, "Refunds", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(refundLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CompleteRefundAsync(refundId, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(RefundStatus.Completed);
    }

    #endregion

    #region GenerateRefundNumberAsync

    [Fact]
    public async Task GenerateRefundNumberAsync_ReturnsFormattedNumber()
    {
        _refundRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Refund, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(7);

        var result = await _service.GenerateRefundNumberAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Match("RFN-20260730-00008");
    }

    #endregion

    #region GetRefundHistoryAsync

    [Fact]
    public async Task GetRefundHistoryAsync_ReturnsRefundsForPayment()
    {
        var paymentId = Guid.NewGuid();
        var refunds = new List<Refund>
        {
            new() { Id = Guid.NewGuid(), PaymentId = paymentId, TotalAmount = 100m, Status = RefundStatus.Completed, RefundNumber = "RFN-001" }
        };
        _refundRepoMock.Setup(r => r.GetByPaymentIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refunds);

        var result = await _service.GetRefundHistoryAsync(paymentId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetRefundHistoryAsync_NoRefunds_ReturnsEmptyList()
    {
        _refundRepoMock.Setup(r => r.GetByPaymentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Refund>());

        var result = await _service.GetRefundHistoryAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion
}
