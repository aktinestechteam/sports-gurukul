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

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<ICouponRepository> _couponRepoMock;
    private readonly Mock<IDiscountService> _discountServiceMock;
    private readonly Mock<ITaxCalculationService> _taxCalcServiceMock;
    private readonly Mock<ILedgerService> _ledgerServiceMock;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _couponRepoMock = new Mock<ICouponRepository>();
        _discountServiceMock = new Mock<IDiscountService>();
        _taxCalcServiceMock = new Mock<ITaxCalculationService>();
        _ledgerServiceMock = new Mock<ILedgerService>();
        _service = new InvoiceService(
            _invoiceRepoMock.Object,
            _couponRepoMock.Object,
            _discountServiceMock.Object,
            _taxCalcServiceMock.Object,
            _ledgerServiceMock.Object);
    }

    #region CreateInvoiceAsync

    [Fact]
    public async Task CreateInvoiceAsync_ValidRequestWithoutCouponOrScholarship_ReturnsSuccess()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test invoice", DateTime.UtcNow.AddDays(30), "INR",
            new List<CreateInvoiceLineItemDto>
            {
                new("Coaching Fee", "Service", null, 1, 1000m, null)
            },
            null, null);

        var taxItems = new List<TaxLineItem> { new("GST 18%", "GST18", 0.18m, 180m) };
        _taxCalcServiceMock.Setup(t => t.CalculateInvoiceTaxesAsync(It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TaxLineItem>>.Success(taxItems));

        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be(InvoiceStatus.Draft);
        _invoiceRepoMock.Verify(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithCouponCode_AppliesDiscount()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test", DateTime.UtcNow.AddDays(30), "INR",
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 2, 500m, null)
            },
            "SAVE10", null);

        var discountResult = new DiscountResult("SAVE10", 100m, 900m);
        _discountServiceMock.Setup(d => d.ApplyDiscountAsync(It.IsAny<decimal>(), "SAVE10", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(discountResult));

        var taxItems = new List<TaxLineItem> { new("GST 18%", "GST18", 0.18m, 162m) };
        _taxCalcServiceMock.Setup(t => t.CalculateInvoiceTaxesAsync(900m, "INR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TaxLineItem>>.Success(taxItems));

        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _discountServiceMock.Verify(d => d.ApplyDiscountAsync(1000m, "SAVE10", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_DiscountServiceFails_ReturnsFailure()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test", null, null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 1, 1000m, null)
            },
            "INVALID", null);

        _discountServiceMock.Setup(d => d.ApplyDiscountAsync(It.IsAny<decimal>(), "INVALID", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Failure("Coupon not found"));

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithScholarshipId_AppliesScholarship()
    {
        var scholarshipId = Guid.NewGuid();
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test", null, null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 1, 1000m, null)
            },
            null, scholarshipId);

        var scholarshipResult = new DiscountResult("Scholarship", 250m, 750m);
        _discountServiceMock.Setup(d => d.ApplyScholarshipAsync(1000m, scholarshipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Success(scholarshipResult));

        var taxItems = new List<TaxLineItem> { new("GST 18%", "GST18", 0.18m, 135m) };
        _taxCalcServiceMock.Setup(t => t.CalculateInvoiceTaxesAsync(750m, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TaxLineItem>>.Success(taxItems));

        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _discountServiceMock.Verify(d => d.ApplyScholarshipAsync(1000m, scholarshipId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_ScholarshipFails_ReturnsFailure()
    {
        var scholarshipId = Guid.NewGuid();
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test", null, null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 1, 1000m, null)
            },
            null, scholarshipId);

        _discountServiceMock.Setup(d => d.ApplyScholarshipAsync(1000m, scholarshipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DiscountResult>.Failure("Scholarship not found"));

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Scholarship not found");
    }

    [Fact]
    public async Task CreateInvoiceAsync_TaxCalculationFails_ReturnsFailure()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), "Test", null, null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 1, 1000m, null)
            },
            null, null);

        _taxCalcServiceMock.Setup(t => t.CalculateInvoiceTaxesAsync(It.IsAny<decimal>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TaxLineItem>>.Failure("Tax calculation failed"));

        var result = await _service.CreateInvoiceAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tax calculation failed");
    }

    [Fact]
    public async Task CreateInvoiceAsync_DefaultValues_WhenOptionalsNotProvided()
    {
        var request = new CreateInvoiceRequest(
            Guid.NewGuid(), Guid.NewGuid(), null, null, null,
            new List<CreateInvoiceLineItemDto>
            {
                new("Fee", "Service", null, 1, 1000m, null)
            },
            null, null);

        var taxItems = new List<TaxLineItem> { new("GST 18%", "GST18", 0.18m, 180m) };
        _taxCalcServiceMock.Setup(t => t.CalculateInvoiceTaxesAsync(1000m, "INR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<TaxLineItem>>.Success(taxItems));

        Invoice? capturedInvoice = null;
        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback((Invoice inv, CancellationToken _) => capturedInvoice = inv)
            .ReturnsAsync((Invoice inv, CancellationToken _) => inv);

        await _service.CreateInvoiceAsync(request, CancellationToken.None);

        capturedInvoice.Should().NotBeNull();
        capturedInvoice!.Currency.Should().Be("INR");
        capturedInvoice.DueDate.Date.Should().Be(DateTime.UtcNow.AddDays(30).Date);
        capturedInvoice.Notes.Should().BeNull();
    }

    #endregion

    #region UpdateInvoiceAsync

    [Fact]
    public async Task UpdateInvoiceAsync_ValidRequest_ReturnsUpdatedInvoice()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Status = InvoiceStatus.Draft,
            Notes = "Original",
            SubTotal = 1000m,
            DiscountTotal = 0,
            TaxTotal = 0,
            Total = 1000m,
            AmountDue = 1000m,
            Items = new List<InvoiceItem>()
        };

        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var request = new UpdateInvoiceRequest("Updated notes", DateTime.UtcNow.AddDays(60),
            new List<CreateInvoiceLineItemDto>
            {
                new("New Item", "Service", null, 2, 500m, null)
            });

        var result = await _service.UpdateInvoiceAsync(invoiceId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be("Updated notes");
        _invoiceRepoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public async Task UpdateInvoiceAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var request = new UpdateInvoiceRequest("Notes", null, null);
        var result = await _service.UpdateInvoiceAsync(Guid.NewGuid(), request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task UpdateInvoiceAsync_NotDraftStatus_ReturnsFailure()
    {
        var invoice = new Invoice { Id = Guid.NewGuid(), Status = InvoiceStatus.Issued };
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var request = new UpdateInvoiceRequest("Notes", null, null);
        var result = await _service.UpdateInvoiceAsync(invoice.Id, request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only draft invoices can be updated");
    }

    [Fact]
    public async Task UpdateInvoiceAsync_OnlyDescriptionUpdate_DoesNotChangeLineItems()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Status = InvoiceStatus.Draft,
            Notes = "Original",
            Items = new List<InvoiceItem>
            {
                new() { Description = "Existing", Quantity = 1, UnitPrice = 500, TotalAmount = 500 }
            },
            SubTotal = 500m,
            DiscountTotal = 0,
            TaxTotal = 0,
            Total = 500m,
            AmountDue = 500m
        };

        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var request = new UpdateInvoiceRequest("Only notes changed", null, null);
        var result = await _service.UpdateInvoiceAsync(invoiceId, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _invoiceRepoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Once);
    }

    #endregion

    #region IssueInvoiceAsync

    [Fact]
    public async Task IssueInvoiceAsync_ValidDraftInvoice_ReturnsIssuedInvoice()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Status = InvoiceStatus.Draft,
            InvoiceNumber = string.Empty,
            SubTotal = 1000m,
            TaxTotal = 0,
            Total = 1000m
        };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Invoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var arLedger = new Ledger { Id = Guid.NewGuid(), Code = "AR" };
        var revLedger = new Ledger { Id = Guid.NewGuid(), Code = "REV" };
        var taxLedger = new Ledger { Id = Guid.NewGuid(), Code = "TAX" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("AR", "Accounts Receivable", LedgerType.Asset, "Accounts Receivable", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(arLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("REV", "Revenue", LedgerType.Income, "Revenue from operations", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(revLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("TAX", "Tax Payable", LedgerType.Liability, "Tax Payable", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(taxLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.IssueInvoiceAsync(invoiceId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Issued);
        result.Value.InvoiceNumber.Should().Be("INV-20260730-00006");
        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task IssueInvoiceAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.IssueInvoiceAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task IssueInvoiceAsync_NotDraftStatus_ReturnsFailure()
    {
        var invoice = new Invoice { Id = Guid.NewGuid(), Status = InvoiceStatus.Issued };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.IssueInvoiceAsync(invoice.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only draft invoices can be issued");
    }

    #endregion

    #region CancelInvoiceAsync

    [Fact]
    public async Task CancelInvoiceAsync_NonPaidInvoice_ReturnsCancelled()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Issued };

        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.CancelInvoiceAsync(invoiceId, "Customer request", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Cancelled);
        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task CancelInvoiceAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.CancelInvoiceAsync(Guid.NewGuid(), "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task CancelInvoiceAsync_PaidInvoice_ReturnsFailure()
    {
        var invoice = new Invoice { Id = Guid.NewGuid(), Status = InvoiceStatus.Paid };
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.CancelInvoiceAsync(invoice.Id, "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a paid invoice");
    }

    #endregion

    #region MarkAsPaidAsync

    [Fact]
    public async Task MarkAsPaidAsync_IssuedInvoice_ReturnsPaid()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            Status = InvoiceStatus.Issued,
            Total = 1000m,
            AmountPaid = 0,
            AmountDue = 1000m
        };

        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.MarkAsPaidAsync(invoiceId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Paid);
        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task MarkAsPaidAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.MarkAsPaidAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task MarkAsPaidAsync_NotIssuedStatus_ReturnsFailure()
    {
        var invoice = new Invoice { Id = Guid.NewGuid(), Status = InvoiceStatus.Draft };
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.MarkAsPaidAsync(invoice.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only issued invoices can be marked as paid");
    }

    #endregion

    #region VoidInvoiceAsync

    [Fact]
    public async Task VoidInvoiceAsync_ValidInvoice_ReturnsVoided()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, Status = InvoiceStatus.Issued, Notes = null };

        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.VoidInvoiceAsync(invoiceId, "Void reason", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(InvoiceStatus.Cancelled);
        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact]
    public async Task VoidInvoiceAsync_AlreadyCancelled_ReturnsFailure()
    {
        var invoice = new Invoice { Id = Guid.NewGuid(), Status = InvoiceStatus.Cancelled };
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.VoidInvoiceAsync(invoice.Id, "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice is already voided");
    }

    [Fact]
    public async Task VoidInvoiceAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.VoidInvoiceAsync(Guid.NewGuid(), "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    #endregion

    #region GenerateInvoiceNumberAsync

    [Fact]
    public async Task GenerateInvoiceNumberAsync_ReturnsFormattedNumber()
    {
        _invoiceRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Invoice, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(42);

        var result = await _service.GenerateInvoiceNumberAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Match("INV-20260730-00043");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_InvoiceExists_ReturnsDto()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice { Id = invoiceId, InvoiceNumber = "INV-001", Status = InvoiceStatus.Draft };
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var result = await _service.GetByIdAsync(invoiceId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(invoiceId);
    }

    [Fact]
    public async Task GetByIdAsync_InvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    #endregion

    #region SearchInvoicesAsync

    [Fact]
    public async Task SearchInvoicesAsync_ByStatus_FiltersCorrectly()
    {
        var request = new InvoiceSearchRequest(null, InvoiceStatus.Paid, null, null, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow }
        };
        _invoiceRepoMock.Setup(r => r.GetByStatusAsync(InvoiceStatus.Paid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchInvoicesAsync_ByAthleteId_FiltersCorrectly()
    {
        var athleteId = Guid.NewGuid();
        var request = new InvoiceSearchRequest(null, null, athleteId, null, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow }
        };
        _invoiceRepoMock.Setup(r => r.GetByAthleteIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchInvoicesAsync_ByAcademyId_FiltersCorrectly()
    {
        var academyId = Guid.NewGuid();
        var request = new InvoiceSearchRequest(null, null, null, academyId, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow }
        };
        _invoiceRepoMock.Setup(r => r.GetByAcademyIdAsync(academyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchInvoicesAsync_NoFilters_ReturnsAll()
    {
        var request = new InvoiceSearchRequest(null, null, null, null, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchInvoicesAsync_WithSearchTerm_FiltersByInvoiceNumber()
    {
        var request = new InvoiceSearchRequest("INV-001", null, null, null, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow, Notes = "Something" },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, Notes = "Else" }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].InvoiceNumber.Should().Be("INV-001");
    }

    [Fact]
    public async Task SearchInvoicesAsync_WithSearchTerm_FiltersByNotes()
    {
        var request = new InvoiceSearchRequest("Something", null, null, null, null, null, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow, Notes = "Something here" },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, Notes = "Other" }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchInvoicesAsync_WithDateRange_FiltersCorrectly()
    {
        var from = DateTime.UtcNow.AddDays(-5);
        var to = DateTime.UtcNow;
        var request = new InvoiceSearchRequest(null, null, null, null, from, to, 1, 20);
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-002", Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow.AddDays(-10) }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchInvoicesAsync_WithPagination_ReturnsCorrectPage()
    {
        var invoices = Enumerable.Range(1, 5).Select(i => new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-{i:D3}",
            Status = InvoiceStatus.Issued,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var request = new InvoiceSearchRequest(null, null, null, null, null, null, 2, 2);
        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].InvoiceNumber.Should().Be("INV-003");
    }

    [Fact]
    public async Task SearchInvoicesAsync_EmptyResult_ReturnsEmptyList()
    {
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Invoice>());

        var request = new InvoiceSearchRequest(null, null, null, null, null, null, 1, 20);
        var result = await _service.SearchInvoicesAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion

    #region GetOutstandingInvoicesAsync

    [Fact]
    public async Task GetOutstandingInvoicesAsync_ReturnsOverdueInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Status = InvoiceStatus.Overdue, CreatedAt = DateTime.UtcNow }
        };
        _invoiceRepoMock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _service.GetOutstandingInvoicesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOutstandingInvoicesAsync_NoOverdue_ReturnsEmptyList()
    {
        _invoiceRepoMock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Invoice>());

        var result = await _service.GetOutstandingInvoicesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion
}
