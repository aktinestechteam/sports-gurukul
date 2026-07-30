using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class InitiatePaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly InitiatePaymentCommandHandler _handler;

    public InitiatePaymentCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new InitiatePaymentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var invoiceId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var command = new InitiatePaymentCommand(invoiceId, 500m, PaymentMethod.UPI, "idem-001", "Test payment");
        var expected = Result<PaymentDto>.Success(new PaymentDto(paymentId, invoiceId, "PAY-001", 500m, null, null, 500m, PaymentMethod.UPI, PaymentStatus.Pending, "idem-001", null, null, null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new InitiatePaymentCommand(Guid.NewGuid(), 500m, PaymentMethod.Card, null, null);
        _serviceMock.Setup(s => s.InitiatePaymentAsync(It.IsAny<InitiatePaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Initiation failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Initiation failed");
    }
}

public class AuthorizePaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly AuthorizePaymentCommandHandler _handler;

    public AuthorizePaymentCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new AuthorizePaymentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentId = Guid.NewGuid();
        var command = new AuthorizePaymentCommand(paymentId);
        var expected = Result<PaymentDto>.Success(new PaymentDto(paymentId, Guid.NewGuid(), "PAY-001", 500m, null, null, 500m, PaymentMethod.Card, PaymentStatus.Authorized, null, null, null, null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.AuthorizePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.AuthorizePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new AuthorizePaymentCommand(Guid.NewGuid());
        _serviceMock.Setup(s => s.AuthorizePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Authorization failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Authorization failed");
    }
}

public class CapturePaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly CapturePaymentCommandHandler _handler;

    public CapturePaymentCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new CapturePaymentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentId = Guid.NewGuid();
        var command = new CapturePaymentCommand(paymentId);
        var expected = Result<PaymentDto>.Success(new PaymentDto(paymentId, Guid.NewGuid(), "PAY-001", 500m, null, null, 500m, PaymentMethod.Card, PaymentStatus.Captured, null, "GW-REF", null, DateTime.UtcNow, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.CapturePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CapturePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CapturePaymentCommand(Guid.NewGuid());
        _serviceMock.Setup(s => s.CapturePaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Capture failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Capture failed");
    }
}

public class CancelPaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly CancelPaymentCommandHandler _handler;

    public CancelPaymentCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new CancelPaymentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentId = Guid.NewGuid();
        var command = new CancelPaymentCommand(paymentId, "Customer cancelled");
        var expected = Result<PaymentDto>.Success(new PaymentDto(paymentId, Guid.NewGuid(), "PAY-001", 500m, null, null, 500m, PaymentMethod.UPI, PaymentStatus.Cancelled, null, null, "Customer cancelled", null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.CancelPaymentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CancelPaymentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CancelPaymentCommand(Guid.NewGuid(), "Reason");
        _serviceMock.Setup(s => s.CancelPaymentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Cancel failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cancel failed");
    }
}

public class RetryPaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly RetryPaymentCommandHandler _handler;

    public RetryPaymentCommandHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new RetryPaymentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var paymentId = Guid.NewGuid();
        var command = new RetryPaymentCommand(paymentId);
        var expected = Result<PaymentDto>.Success(new PaymentDto(paymentId, Guid.NewGuid(), "PAY-002", 500m, null, null, 500m, PaymentMethod.Card, PaymentStatus.Pending, null, null, null, null, null, DateTime.UtcNow));
        _serviceMock.Setup(s => s.RetryPaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.RetryPaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new RetryPaymentCommand(Guid.NewGuid());
        _serviceMock.Setup(s => s.RetryPaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaymentDto>.Failure("Retry failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Retry failed");
    }
}
