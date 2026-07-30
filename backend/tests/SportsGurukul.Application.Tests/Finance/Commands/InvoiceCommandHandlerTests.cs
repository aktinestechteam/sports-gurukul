using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Commands;

public class CreateInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly CreateInvoiceCommandHandler _handler;

    public CreateInvoiceCommandHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new CreateInvoiceCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var invoiceId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var academyId = Guid.NewGuid();
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Service", null, 1, 1000m, null)
        };
        var command = new CreateInvoiceCommand("Test invoice", DateTime.UtcNow.AddDays(30), "INR", athleteId, academyId, lineItems, null, null);
        var expected = Result<InvoiceDto>.Success(new InvoiceDto(invoiceId, "INV-001", athleteId, academyId, "Athlete", "Academy", "Test invoice", 1000m, 0m, 0m, 1000m, 0m, 1000m, InvoiceStatus.Draft, DateTime.UtcNow.AddDays(30), null, null, null, null, "INR", DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>()));
        _serviceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected.Value);
        _serviceMock.Verify(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CreateInvoiceCommand(null, null, null, null, null, new List<CreateInvoiceLineItemDto>(), null, null);
        _serviceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Service error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Service error");
    }
}

public class IssueInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly IssueInvoiceCommandHandler _handler;

    public IssueInvoiceCommandHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new IssueInvoiceCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var invoiceId = Guid.NewGuid();
        var command = new IssueInvoiceCommand(invoiceId);
        var expected = Result<InvoiceDto>.Success(new InvoiceDto(invoiceId, "INV-001", null, null, null, null, null, 0m, 0m, 0m, 0m, 0m, 0m, InvoiceStatus.Issued, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>()));
        _serviceMock.Setup(s => s.IssueInvoiceAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.IssueInvoiceAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new IssueInvoiceCommand(Guid.NewGuid());
        _serviceMock.Setup(s => s.IssueInvoiceAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Issue failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Issue failed");
    }
}

public class CancelInvoiceCommandHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly CancelInvoiceCommandHandler _handler;

    public CancelInvoiceCommandHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new CancelInvoiceCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var invoiceId = Guid.NewGuid();
        var command = new CancelInvoiceCommand(invoiceId, "Customer request");
        var expected = Result<InvoiceDto>.Success(new InvoiceDto(invoiceId, "INV-001", null, null, null, null, null, 0m, 0m, 0m, 0m, 0m, 0m, InvoiceStatus.Cancelled, null, null, null, DateTime.UtcNow, "Customer request", null, DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>()));
        _serviceMock.Setup(s => s.CancelInvoiceAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.CancelInvoiceAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CancelInvoiceCommand(Guid.NewGuid(), "Reason");
        _serviceMock.Setup(s => s.CancelInvoiceAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Cancel failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cancel failed");
    }
}

public class MarkInvoiceAsPaidCommandHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly MarkInvoiceAsPaidCommandHandler _handler;

    public MarkInvoiceAsPaidCommandHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new MarkInvoiceAsPaidCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var invoiceId = Guid.NewGuid();
        var command = new MarkInvoiceAsPaidCommand(invoiceId);
        var expected = Result<InvoiceDto>.Success(new InvoiceDto(invoiceId, "INV-001", null, null, null, null, null, 100m, 0m, 0m, 100m, 100m, 0m, InvoiceStatus.Paid, null, null, DateTime.UtcNow, null, null, null, DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>()));
        _serviceMock.Setup(s => s.MarkAsPaidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.MarkAsPaidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new MarkInvoiceAsPaidCommand(Guid.NewGuid());
        _serviceMock.Setup(s => s.MarkAsPaidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Mark as paid failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Mark as paid failed");
    }
}
