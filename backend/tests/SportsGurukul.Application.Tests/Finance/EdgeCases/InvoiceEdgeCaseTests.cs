using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.EdgeCases;

public class CreateInvoiceEdgeCaseTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly CreateInvoiceCommandHandler _handler;

    public CreateInvoiceEdgeCaseTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _handler = new CreateInvoiceCommandHandler(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task CreateInvoice_MaximumLineItems_ShouldSucceed()
    {
        var lineItems = Enumerable.Range(1, 100).Select(i =>
            new CreateInvoiceLineItemDto($"Item {i}", "Service", null, 1, 100m, null)
        ).ToList();

        var command = new CreateInvoiceCommand("Bulk invoice", DateTime.UtcNow.AddDays(30), "INR", Guid.NewGuid(), Guid.NewGuid(), lineItems, null, null);

        var invoiceDto = new InvoiceDto(Guid.NewGuid(), "INV-001", null, null, null, null, "Bulk invoice",
            10000m, 1800m, 0m, 11800m, 0m, 11800m, InvoiceStatus.Draft, DateTime.UtcNow.AddDays(30),
            null, null, null, null, "INR", DateTime.UtcNow,
            lineItems.Select((li, idx) => new InvoiceLineItemDto(Guid.NewGuid(), li.Description, li.ItemType, li.ItemReference, li.Quantity, li.UnitPrice, li.Quantity * li.UnitPrice, null, null, li.Quantity * li.UnitPrice)).ToList(),
            new List<InvoicePaymentDto>());

        _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(invoiceDto));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LineItems.Should().HaveCount(100);
        _invoiceServiceMock.Verify(s => s.CreateInvoiceAsync(It.Is<CreateInvoiceRequest>(r => r.LineItems.Count == 100), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoice_ZeroTotal_ShouldSucceed()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Free item", "Service", null, 0, 0m, null)
        };

        var command = new CreateInvoiceCommand("Zero value invoice", DateTime.UtcNow.AddDays(30), "INR", Guid.NewGuid(), Guid.NewGuid(), lineItems, null, null);

        var invoiceDto = new InvoiceDto(Guid.NewGuid(), "INV-002", null, null, null, null, "Zero value invoice",
            0m, 0m, 0m, 0m, 0m, 0m, InvoiceStatus.Draft, DateTime.UtcNow.AddDays(30),
            null, null, null, null, "INR", DateTime.UtcNow,
            new List<InvoiceLineItemDto>(),
            new List<InvoicePaymentDto>());

        _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(invoiceDto));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(0m);
        _invoiceServiceMock.Verify(s => s.CreateInvoiceAsync(It.Is<CreateInvoiceRequest>(r => r.LineItems[0].Quantity == 0), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoice_VeryLargeAmounts_ShouldSucceed()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Large fee", "Service", null, 1, 999_999_999.99m, null),
            new("Another large fee", "Service", null, 2, 500_000_000.50m, null)
        };

        var command = new CreateInvoiceCommand("Large invoice", DateTime.UtcNow.AddDays(30), "INR", Guid.NewGuid(), Guid.NewGuid(), lineItems, null, null);

        var invoiceDto = new InvoiceDto(Guid.NewGuid(), "INV-003", null, null, null, null, "Large invoice",
            2_000_000_000.99m, 360_000_000.18m, 0m, 2_360_000_001.17m, 0m, 2_360_000_001.17m,
            InvoiceStatus.Draft, DateTime.UtcNow.AddDays(30), null, null, null, null, "INR", DateTime.UtcNow,
            new List<InvoiceLineItemDto>(),
            new List<InvoicePaymentDto>());

        _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(invoiceDto));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().BeGreaterThan(2_000_000_000m);
        _invoiceServiceMock.Verify(s => s.CreateInvoiceAsync(It.Is<CreateInvoiceRequest>(r => r.LineItems.Count == 2), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoice_ServiceFailure_ReturnsFailure()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Fee", "Service", null, 1, 1000m, null)
        };

        var command = new CreateInvoiceCommand(null, null, null, null, null, lineItems, null, null);

        _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Failed to create invoice"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to create invoice");
    }
}

public class IssueInvoiceEdgeCaseTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly IssueInvoiceCommandHandler _handler;

    public IssueInvoiceEdgeCaseTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _handler = new IssueInvoiceCommandHandler(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task IssueInvoice_AlreadyIssuedInvoice_ShouldFail()
    {
        var invoiceId = Guid.NewGuid();
        var command = new IssueInvoiceCommand(invoiceId);

        _invoiceServiceMock.Setup(s => s.IssueInvoiceAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Invoice is already issued"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice is already issued");
        _invoiceServiceMock.Verify(s => s.IssueInvoiceAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IssueInvoice_CancelledInvoice_ShouldFail()
    {
        var invoiceId = Guid.NewGuid();
        var command = new IssueInvoiceCommand(invoiceId);

        _invoiceServiceMock.Setup(s => s.IssueInvoiceAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Cannot issue a cancelled invoice"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot issue a cancelled invoice");
        _invoiceServiceMock.Verify(s => s.IssueInvoiceAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class CancelInvoiceEdgeCaseTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly CancelInvoiceCommandHandler _handler;

    public CancelInvoiceEdgeCaseTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _handler = new CancelInvoiceCommandHandler(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task CancelInvoice_PaidInvoice_ShouldFail()
    {
        var invoiceId = Guid.NewGuid();
        var command = new CancelInvoiceCommand(invoiceId, "Customer request");

        _invoiceServiceMock.Setup(s => s.CancelInvoiceAsync(invoiceId, "Customer request", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Cannot cancel a paid invoice"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a paid invoice");
        _invoiceServiceMock.Verify(s => s.CancelInvoiceAsync(invoiceId, "Customer request", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class MarkInvoiceAsPaidEdgeCaseTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly MarkInvoiceAsPaidCommandHandler _handler;

    public MarkInvoiceAsPaidEdgeCaseTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _handler = new MarkInvoiceAsPaidCommandHandler(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task MarkAsPaid_DraftInvoice_ShouldFail()
    {
        var invoiceId = Guid.NewGuid();
        var command = new MarkInvoiceAsPaidCommand(invoiceId);

        _invoiceServiceMock.Setup(s => s.MarkAsPaidAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Cannot mark a draft invoice as paid"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot mark a draft invoice as paid");
        _invoiceServiceMock.Verify(s => s.MarkAsPaidAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsPaid_AlreadyPaidInvoice_ShouldFail()
    {
        var invoiceId = Guid.NewGuid();
        var command = new MarkInvoiceAsPaidCommand(invoiceId);

        _invoiceServiceMock.Setup(s => s.MarkAsPaidAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Invoice is already marked as paid"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice is already marked as paid");
        _invoiceServiceMock.Verify(s => s.MarkAsPaidAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
