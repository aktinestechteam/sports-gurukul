using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class RequestRefundCommandHandlerTests
{
    private readonly Mock<IRefundService> _serviceMock;
    private readonly RequestRefundCommandHandler _handler;

    public RequestRefundCommandHandlerTests()
    {
        _serviceMock = new Mock<IRefundService>();
        _handler = new RequestRefundCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentId = Guid.NewGuid();
        var refundId = Guid.NewGuid();
        var items = new List<RefundItemRequest>
        {
            new("Item 1", 100m)
        };
        var command = new RequestRefundCommand(paymentId, 100m, "Defective item", items);
        var expected = Result<RefundDto>.Success(new RefundDto(refundId, paymentId, "RFN-001", 100m, "Defective item", RefundStatus.Requested, null, null, null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.RequestRefundAsync(It.IsAny<RequestRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.RequestRefundAsync(It.IsAny<RequestRefundRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new RequestRefundCommand(Guid.NewGuid(), 100m, null, null);
        _serviceMock.Setup(s => s.RequestRefundAsync(It.IsAny<RequestRefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Refund request failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Refund request failed");
    }
}

public class ApproveRefundCommandHandlerTests
{
    private readonly Mock<IRefundService> _serviceMock;
    private readonly ApproveRefundCommandHandler _handler;

    public ApproveRefundCommandHandlerTests()
    {
        _serviceMock = new Mock<IRefundService>();
        _handler = new ApproveRefundCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var refundId = Guid.NewGuid();
        var command = new ApproveRefundCommand(refundId, "Admin");
        var expected = Result<RefundDto>.Success(new RefundDto(refundId, Guid.NewGuid(), "RFN-001", 100m, null, RefundStatus.Approved, "Admin", null, null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.ApproveRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.ApproveRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new ApproveRefundCommand(Guid.NewGuid(), "Admin");
        _serviceMock.Setup(s => s.ApproveRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Approval failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Approval failed");
    }
}

public class RejectRefundCommandHandlerTests
{
    private readonly Mock<IRefundService> _serviceMock;
    private readonly RejectRefundCommandHandler _handler;

    public RejectRefundCommandHandlerTests()
    {
        _serviceMock = new Mock<IRefundService>();
        _handler = new RejectRefundCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var refundId = Guid.NewGuid();
        var command = new RejectRefundCommand(refundId, "Not eligible");
        var expected = Result<RefundDto>.Success(new RefundDto(refundId, Guid.NewGuid(), "RFN-001", 100m, null, RefundStatus.Rejected, null, "Not eligible", null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.RejectRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.RejectRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new RejectRefundCommand(Guid.NewGuid(), "Reason");
        _serviceMock.Setup(s => s.RejectRefundAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Rejection failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Rejection failed");
    }
}

public class CompleteRefundCommandHandlerTests
{
    private readonly Mock<IRefundService> _serviceMock;
    private readonly CompleteRefundCommandHandler _handler;

    public CompleteRefundCommandHandlerTests()
    {
        _serviceMock = new Mock<IRefundService>();
        _handler = new CompleteRefundCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var refundId = Guid.NewGuid();
        var command = new CompleteRefundCommand(refundId, "GW-REF-001");
        var expected = Result<RefundDto>.Success(new RefundDto(refundId, Guid.NewGuid(), "RFN-001", 100m, null, RefundStatus.Completed, null, null, "GW-REF-001", DateTime.UtcNow, DateTime.UtcNow));
        _serviceMock.Setup(s => s.CompleteRefundAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CompleteRefundAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CompleteRefundCommand(Guid.NewGuid(), null);
        _serviceMock.Setup(s => s.CompleteRefundAsync(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RefundDto>.Failure("Completion failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Completion failed");
    }
}
