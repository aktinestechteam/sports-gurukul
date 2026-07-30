using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class InitiatePaymentEdgeCaseTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly InitiatePaymentCommandHandler _handler;

    public InitiatePaymentEdgeCaseTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new InitiatePaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task InitiatePayment_DuplicateIdempotencyKey_ReturnsCachedResult()
    {
        var invoiceId = Guid.NewGuid();
        var idempotencyKey = "idem-dup-001";
        var command = new InitiatePaymentCommand(invoiceId, 500m, PaymentMethod.UPI, idempotencyKey, "Test payment");

        var existingPayment = new PaymentDto(Guid.NewGuid(), invoiceId, "PAY-001", 500m, null, null, 500m,
            PaymentMethod.UPI, PaymentStatus.Pending, idempotencyKey, null, null, null, null, DateTime.UtcNow);

        _paymentServiceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Success(existingPayment));

        var firstResult = await _handler.Handle(command, CancellationToken.None);
        var secondResult = await _handler.Handle(command, CancellationToken.None);

        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();
        firstResult.Value.Id.Should().Be(secondResult.Value.Id);
        firstResult.Value.IdempotencyKey.Should().Be(idempotencyKey);
        secondResult.Value.IdempotencyKey.Should().Be(idempotencyKey);
        _paymentServiceMock.Verify(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task InitiatePayment_PartialPayment_ShouldSucceed()
    {
        var invoiceId = Guid.NewGuid();
        var command = new InitiatePaymentCommand(invoiceId, 300m, PaymentMethod.Card, null, "Partial payment");

        var paymentDto = new PaymentDto(Guid.NewGuid(), invoiceId, "PAY-002", 300m, null, null, 300m,
            PaymentMethod.Card, PaymentStatus.Pending, null, null, null, null, null, DateTime.UtcNow);

        _paymentServiceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Success(paymentDto));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(300m);
        result.Value.Status.Should().Be(PaymentStatus.Pending);
        _paymentServiceMock.Verify(s => s.InitiatePaymentAsync(It.Is<InitiatePaymentRequest>(r => r.Amount == 300m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitiatePayment_Overpayment_ShouldSucceed()
    {
        var invoiceId = Guid.NewGuid();
        var command = new InitiatePaymentCommand(invoiceId, 2000m, PaymentMethod.NetBanking, null, "Overpayment");

        var paymentDto = new PaymentDto(Guid.NewGuid(), invoiceId, "PAY-003", 2000m, null, null, 2000m,
            PaymentMethod.NetBanking, PaymentStatus.Pending, null, null, null, null, null, DateTime.UtcNow);

        _paymentServiceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Success(paymentDto));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(2000m);
        _paymentServiceMock.Verify(s => s.InitiatePaymentAsync(It.Is<InitiatePaymentRequest>(r => r.Amount == 2000m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitiatePayment_GatewayFailureDuringCapture_ReturnsFailure()
    {
        var invoiceId = Guid.NewGuid();
        var command = new InitiatePaymentCommand(invoiceId, 500m, PaymentMethod.Card, null, "Gateway test");

        _paymentServiceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Gateway declined the transaction"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Gateway declined the transaction");
        _paymentServiceMock.Verify(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AuthorizePaymentEdgeCaseTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly AuthorizePaymentCommandHandler _handler;

    public AuthorizePaymentEdgeCaseTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new AuthorizePaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task AuthorizePayment_AlreadyAuthorized_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new AuthorizePaymentCommand(paymentId);

        _paymentServiceMock.Setup(s => s.AuthorizePaymentAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Payment is already authorized"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment is already authorized");
        _paymentServiceMock.Verify(s => s.AuthorizePaymentAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CapturePaymentEdgeCaseTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly CapturePaymentCommandHandler _handler;

    public CapturePaymentEdgeCaseTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new CapturePaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task CapturePayment_PendingPayment_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new CapturePaymentCommand(paymentId);

        _paymentServiceMock.Setup(s => s.CapturePaymentAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Cannot capture a pending payment. Authorize first."));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot capture a pending payment. Authorize first.");
        _paymentServiceMock.Verify(s => s.CapturePaymentAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CapturePayment_GatewayFailure_ReturnsFailure()
    {
        var paymentId = Guid.NewGuid();
        var command = new CapturePaymentCommand(paymentId);

        _paymentServiceMock.Setup(s => s.CapturePaymentAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Gateway capture failed: insufficient funds"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Gateway capture failed: insufficient funds");
        _paymentServiceMock.Verify(s => s.CapturePaymentAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CancelPaymentEdgeCaseTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly CancelPaymentCommandHandler _handler;

    public CancelPaymentEdgeCaseTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new CancelPaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task CancelPayment_AlreadyCaptured_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new CancelPaymentCommand(paymentId, "Customer changed mind");

        _paymentServiceMock.Setup(s => s.CancelPaymentAsync(paymentId, "Customer changed mind", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Cannot cancel a captured payment"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a captured payment");
        _paymentServiceMock.Verify(s => s.CancelPaymentAsync(paymentId, "Customer changed mind", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class RetryPaymentEdgeCaseTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly RetryPaymentCommandHandler _handler;

    public RetryPaymentEdgeCaseTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new RetryPaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task RetryPayment_NonFailedPayment_ShouldFail()
    {
        var paymentId = Guid.NewGuid();
        var command = new RetryPaymentCommand(paymentId);

        _paymentServiceMock.Setup(s => s.RetryPaymentAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Only failed payments can be retried"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only failed payments can be retried");
        _paymentServiceMock.Verify(s => s.RetryPaymentAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
