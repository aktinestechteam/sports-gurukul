using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class RequestRefundEdgeCaseTests
{
    private readonly Mock<IRefundService> _refundServiceMock;
    private readonly RequestRefundCommandHandler _handler;

    public RequestRefundEdgeCaseTests()
    {
        _refundServiceMock = new Mock<IRefundService>();
        _handler = new RequestRefundCommandHandler(_refundServiceMock.Object);
    }

    [Fact]
    public async Task RequestRefund_OverRefund_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new RequestRefundCommand(paymentId, 5000m, "Over-refund request", null);

        _refundServiceMock.Setup(s => s.RequestRefundAsync(It.IsAny<RequestRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Refund amount exceeds payment amount"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund amount exceeds payment amount");
        _refundServiceMock.Verify(s => s.RequestRefundAsync(It.Is<RequestRefundRequest>(r => r.Amount == 5000m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestRefund_NonExistentPayment_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new RequestRefundCommand(paymentId, 100m, "Refund for missing payment", null);

        _refundServiceMock.Setup(s => s.RequestRefundAsync(It.IsAny<RequestRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Payment not found"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
        _refundServiceMock.Verify(s => s.RequestRefundAsync(It.Is<RequestRefundRequest>(r => r.PaymentId == paymentId), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ApproveRefundEdgeCaseTests
{
    private readonly Mock<IRefundService> _refundServiceMock;
    private readonly ApproveRefundCommandHandler _handler;

    public ApproveRefundEdgeCaseTests()
    {
        _refundServiceMock = new Mock<IRefundService>();
        _handler = new ApproveRefundCommandHandler(_refundServiceMock.Object);
    }

    [Fact]
    public async Task ApproveRefund_AlreadyApproved_ShouldFail()
    {
        var refundId = Guid.NewGuid();
        var command = new ApproveRefundCommand(refundId, "admin@test.com");

        _refundServiceMock.Setup(s => s.ApproveRefundAsync(refundId, "admin@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Refund is already approved"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund is already approved");
        _refundServiceMock.Verify(s => s.ApproveRefundAsync(refundId, "admin@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class RejectRefundEdgeCaseTests
{
    private readonly Mock<IRefundService> _refundServiceMock;
    private readonly RejectRefundCommandHandler _handler;

    public RejectRefundEdgeCaseTests()
    {
        _refundServiceMock = new Mock<IRefundService>();
        _handler = new RejectRefundCommandHandler(_refundServiceMock.Object);
    }

    [Fact]
    public async Task RejectRefund_AlreadyRejected_ShouldFail()
    {
        var refundId = Guid.NewGuid();
        var command = new RejectRefundCommand(refundId, "Already processed");

        _refundServiceMock.Setup(s => s.RejectRefundAsync(refundId, "Already processed", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Refund is already rejected"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund is already rejected");
        _refundServiceMock.Verify(s => s.RejectRefundAsync(refundId, "Already processed", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CompleteRefundEdgeCaseTests
{
    private readonly Mock<IRefundService> _refundServiceMock;
    private readonly CompleteRefundCommandHandler _handler;

    public CompleteRefundEdgeCaseTests()
    {
        _refundServiceMock = new Mock<IRefundService>();
        _handler = new CompleteRefundCommandHandler(_refundServiceMock.Object);
    }

    [Fact]
    public async Task CompleteRefund_NonApprovedRefund_ShouldFail()
    {
        var refundId = Guid.NewGuid();
        var command = new CompleteRefundCommand(refundId, "GATEWAY-REF-001");

        _refundServiceMock.Setup(s => s.CompleteRefundAsync(refundId, "GATEWAY-REF-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Only approved refunds can be completed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only approved refunds can be completed");
        _refundServiceMock.Verify(s => s.CompleteRefundAsync(refundId, "GATEWAY-REF-001", It.IsAny<CancellationToken>()), Times.Once);
    }
}
