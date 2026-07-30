using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Queries;

public class GetInvoiceByIdQueryHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly GetInvoiceByIdQueryHandler _handler;

    public GetInvoiceByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new GetInvoiceByIdQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvoiceDto_WhenInvoiceExists()
    {
        var invoiceId = Guid.NewGuid();
        var dto = new InvoiceDto(invoiceId, "INV-001", null, null, null, null, null, 100, 18, 0, 118, 118, 0,
            InvoiceStatus.Paid, null, null, null, null, null, "INR", DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>());
        _serviceMock.Setup(s => s.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(dto));

        var result = await _handler.Handle(new GetInvoiceByIdQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(invoiceId);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenInvoiceNotFound()
    {
        var invoiceId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Failure("Invoice not found"));

        var result = await _handler.Handle(new GetInvoiceByIdQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectId()
    {
        var invoiceId = Guid.NewGuid();
        var dto = new InvoiceDto(invoiceId, "INV-001", null, null, null, null, null, 100, 18, 0, 118, 118, 0,
            InvoiceStatus.Paid, null, null, null, null, null, "INR", DateTime.UtcNow, new List<InvoiceLineItemDto>(), new List<InvoicePaymentDto>());
        _serviceMock.Setup(s => s.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<InvoiceDto>.Success(dto));

        await _handler.Handle(new GetInvoiceByIdQuery(invoiceId), CancellationToken.None);

        _serviceMock.Verify(s => s.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class SearchInvoicesQueryHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly SearchInvoicesQueryHandler _handler;

    public SearchInvoicesQueryHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new SearchInvoicesQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvoices_WhenSearchTermProvided()
    {
        var query = new SearchInvoicesQuery("INV", null, null, null, null, null, 1, 20);
        var summaries = new List<InvoiceSummaryDto>
        {
            new(Guid.NewGuid(), "INV-001", "Athlete", 500, 500, 0, InvoiceStatus.Paid, null, DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.SearchInvoicesAsync(It.IsAny<InvoiceSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(summaries));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoMatches()
    {
        var query = new SearchInvoicesQuery("NONEXISTENT", null, null, null, null, null, 1, 20);
        _serviceMock.Setup(s => s.SearchInvoicesAsync(It.IsAny<InvoiceSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(new List<InvoiceSummaryDto>()));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceWithSearchRequest()
    {
        var query = new SearchInvoicesQuery("INV", InvoiceStatus.Paid, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, 1, 20);
        _serviceMock.Setup(s => s.SearchInvoicesAsync(It.IsAny<InvoiceSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(new List<InvoiceSummaryDto>()));

        await _handler.Handle(query, CancellationToken.None);

        _serviceMock.Verify(s => s.SearchInvoicesAsync(
            It.Is<InvoiceSearchRequest>(r => r.SearchTerm == "INV" && r.Status == InvoiceStatus.Paid && r.Page == 1 && r.PageSize == 20),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetOutstandingInvoicesQueryHandlerTests
{
    private readonly Mock<IInvoiceService> _serviceMock;
    private readonly GetOutstandingInvoicesQueryHandler _handler;

    public GetOutstandingInvoicesQueryHandlerTests()
    {
        _serviceMock = new Mock<IInvoiceService>();
        _handler = new GetOutstandingInvoicesQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOutstandingInvoices()
    {
        var invoices = new List<InvoiceSummaryDto>
        {
            new(Guid.NewGuid(), "INV-002", "Athlete", 1000, 200, 800, InvoiceStatus.Issued, DateTime.UtcNow.AddDays(30), DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetOutstandingInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(invoices));

        var result = await _handler.Handle(new GetOutstandingInvoicesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoOutstandingInvoices()
    {
        _serviceMock.Setup(s => s.GetOutstandingInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(new List<InvoiceSummaryDto>()));

        var result = await _handler.Handle(new GetOutstandingInvoicesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceMethod()
    {
        _serviceMock.Setup(s => s.GetOutstandingInvoicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<InvoiceSummaryDto>>.Success(new List<InvoiceSummaryDto>()));

        await _handler.Handle(new GetOutstandingInvoicesQuery(), CancellationToken.None);

        _serviceMock.Verify(s => s.GetOutstandingInvoicesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetRevenueQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _repoMock;
    private readonly GetRevenueQueryHandler _handler;

    public GetRevenueQueryHandlerTests()
    {
        _repoMock = new Mock<IInvoiceRepository>();
        _handler = new GetRevenueQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCalculatedRevenue_WithPaidInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 1000, TaxTotal = 180, DiscountTotal = 0, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, InvoiceNumber = "INV-001", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 500, TaxTotal = 90, DiscountTotal = 50, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, InvoiceNumber = "INV-002", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 300, TaxTotal = 0, DiscountTotal = 0, Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow, InvoiceNumber = "INV-003", Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _handler.Handle(new GetRevenueQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(1500);
        result.Value.TotalTax.Should().Be(270);
        result.Value.TotalDiscount.Should().Be(50);
        result.Value.NetRevenue.Should().Be(1230);
        result.Value.InvoiceCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ReturnsZeroRevenue_WhenNoPaidInvoices()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 300, TaxTotal = 0, DiscountTotal = 0, Status = InvoiceStatus.Draft, CreatedAt = DateTime.UtcNow, InvoiceNumber = "INV-001", Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await _handler.Handle(new GetRevenueQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(0);
        result.Value.InvoiceCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_FiltersByDateRange()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 1000, TaxTotal = 180, DiscountTotal = 0, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow.AddDays(-5), InvoiceNumber = "INV-001", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 500, TaxTotal = 90, DiscountTotal = 0, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow.AddDays(-20), InvoiceNumber = "INV-002", Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var fromDate = DateTime.UtcNow.AddDays(-10);
        var toDate = DateTime.UtcNow;
        var result = await _handler.Handle(new GetRevenueQuery(fromDate, toDate), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(1000);
        result.Value.InvoiceCount.Should().Be(1);
    }
}

public class GetPaymentHistoryQueryHandlerTests
{
    private readonly Mock<IPaymentService> _serviceMock;
    private readonly GetPaymentHistoryQueryHandler _handler;

    public GetPaymentHistoryQueryHandlerTests()
    {
        _serviceMock = new Mock<IPaymentService>();
        _handler = new GetPaymentHistoryQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaymentHistory()
    {
        var invoiceId = Guid.NewGuid();
        var payments = new List<PaymentDto>
        {
            new(Guid.NewGuid(), invoiceId, "PAY-001", 500, null, null, 500, Domain.Enums.Finance.PaymentMethod.UPI, PaymentStatus.Captured, null, null, null, DateTime.UtcNow, null, DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetPaymentHistoryAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<PaymentDto>>.Success(payments));

        var result = await _handler.Handle(new GetPaymentHistoryQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoPayments()
    {
        var invoiceId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetPaymentHistoryAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<PaymentDto>>.Success(new List<PaymentDto>()));

        var result = await _handler.Handle(new GetPaymentHistoryQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectInvoiceId()
    {
        var invoiceId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetPaymentHistoryAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<PaymentDto>>.Success(new List<PaymentDto>()));

        await _handler.Handle(new GetPaymentHistoryQuery(invoiceId), CancellationToken.None);

        _serviceMock.Verify(s => s.GetPaymentHistoryAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetRefundHistoryQueryHandlerTests
{
    private readonly Mock<IRefundService> _serviceMock;
    private readonly GetRefundHistoryQueryHandler _handler;

    public GetRefundHistoryQueryHandlerTests()
    {
        _serviceMock = new Mock<IRefundService>();
        _handler = new GetRefundHistoryQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRefundHistory()
    {
        var paymentId = Guid.NewGuid();
        var refunds = new List<RefundDto>
        {
            new(Guid.NewGuid(), paymentId, "RFD-001", 100, "Damaged item", RefundStatus.Completed, "Admin", null, "GATEWAY-REF", DateTime.UtcNow, DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetRefundHistoryAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<RefundDto>>.Success(refunds));

        var result = await _handler.Handle(new GetRefundHistoryQuery(paymentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoRefunds()
    {
        var paymentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetRefundHistoryAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<RefundDto>>.Success(new List<RefundDto>()));

        var result = await _handler.Handle(new GetRefundHistoryQuery(paymentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectPaymentId()
    {
        var paymentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetRefundHistoryAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<RefundDto>>.Success(new List<RefundDto>()));

        await _handler.Handle(new GetRefundHistoryQuery(paymentId), CancellationToken.None);

        _serviceMock.Verify(s => s.GetRefundHistoryAsync(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetWalletByUserIdQueryHandlerTests
{
    private readonly Mock<IWalletService> _serviceMock;
    private readonly GetWalletByUserIdQueryHandler _handler;

    public GetWalletByUserIdQueryHandlerTests()
    {
        _serviceMock = new Mock<IWalletService>();
        _handler = new GetWalletByUserIdQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsWalletDto_WhenWalletExists()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();
        var dto = new WalletDto(walletId, userId, 5000, "INR", DateTime.UtcNow, DateTime.UtcNow);
        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Success(dto));

        var result = await _handler.Handle(new GetWalletByUserIdQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenWalletNotFound()
    {
        var userId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Wallet not found"));

        var result = await _handler.Handle(new GetWalletByUserIdQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Wallet not found");
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectUserId()
    {
        var userId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<WalletDto>.Failure("Not found"));

        await _handler.Handle(new GetWalletByUserIdQuery(userId), CancellationToken.None);

        _serviceMock.Verify(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetSettlementByIdQueryHandlerTests
{
    private readonly Mock<ISettlementService> _serviceMock;
    private readonly GetSettlementByIdQueryHandler _handler;

    public GetSettlementByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<ISettlementService>();
        _handler = new GetSettlementByIdQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsSettlementDto_WhenSettlementExists()
    {
        var batchId = Guid.NewGuid();
        var dto = new SettlementDto(batchId, "BATCH-001", 5000, 3, SettlementStatus.Completed, "REF-001", DateTime.UtcNow, DateTime.UtcNow, new List<SettlementItemDto>());
        _serviceMock.Setup(s => s.GetByIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Success(dto));

        var result = await _handler.Handle(new GetSettlementByIdQuery(batchId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(batchId);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenSettlementNotFound()
    {
        var batchId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Settlement not found"));

        var result = await _handler.Handle(new GetSettlementByIdQuery(batchId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Settlement not found");
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectBatchId()
    {
        var batchId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<SettlementDto>.Failure("Not found"));

        await _handler.Handle(new GetSettlementByIdQuery(batchId), CancellationToken.None);

        _serviceMock.Verify(s => s.GetByIdAsync(batchId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetFinanceDashboardQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<ICouponRepository> _couponRepoMock;
    private readonly GetFinanceDashboardQueryHandler _handler;

    public GetFinanceDashboardQueryHandlerTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _couponRepoMock = new Mock<ICouponRepository>();
        _handler = new GetFinanceDashboardQueryHandler(_invoiceRepoMock.Object, _couponRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsDashboardDto_WithCalculatedValues()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 1000, AmountDue = 0, Status = InvoiceStatus.Paid, InvoiceNumber = "INV-001", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 500, AmountDue = 500, Status = InvoiceStatus.Issued, InvoiceNumber = "INV-002", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 200, AmountDue = 200, Status = InvoiceStatus.Issued, InvoiceNumber = "INV-003", Currency = "INR" }
        };
        var overdue = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 200, AmountDue = 200, Status = InvoiceStatus.Overdue, InvoiceNumber = "INV-004", Currency = "INR" }
        };
        var activeCoupons = new List<Coupon>
        {
            new() { Id = Guid.NewGuid(), Code = "SAVE10", IsActive = true }
        };

        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);
        _invoiceRepoMock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(overdue);
        _couponRepoMock.Setup(r => r.GetActiveCouponsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(activeCoupons);

        var result = await _handler.Handle(new GetFinanceDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(1000);
        result.Value.OutstandingAmount.Should().Be(700);
        result.Value.PendingInvoices.Should().Be(2);
        result.Value.OverdueInvoices.Should().Be(1);
        result.Value.RecentPayments.Should().Be(1);
        result.Value.ActiveCoupons.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReturnsZeroValues_WhenNoData()
    {
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Invoice>());
        _invoiceRepoMock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Invoice>());
        _couponRepoMock.Setup(r => r.GetActiveCouponsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Coupon>());

        var result = await _handler.Handle(new GetFinanceDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(0);
        result.Value.OutstandingAmount.Should().Be(0);
        result.Value.PendingInvoices.Should().Be(0);
        result.Value.OverdueInvoices.Should().Be(0);
        result.Value.ActiveCoupons.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CalculatesOverdueAndActiveCoupons()
    {
        var invoices = new List<Invoice>();
        var overdue = new List<Invoice> { new() { Id = Guid.NewGuid(), Status = InvoiceStatus.Overdue, InvoiceNumber = "INV-001", Currency = "INR" } };
        var activeCoupons = new List<Coupon> { new() { Id = Guid.NewGuid(), Code = "DISCOUNT", IsActive = true } };

        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);
        _invoiceRepoMock.Setup(r => r.GetOverdueInvoicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(overdue);
        _couponRepoMock.Setup(r => r.GetActiveCouponsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(activeCoupons);

        var result = await _handler.Handle(new GetFinanceDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.OverdueInvoices.Should().Be(1);
        result.Value.ActiveCoupons.Should().Be(1);
    }
}

public class GetFinanceReportsQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _repoMock;
    private readonly GetFinanceReportsQueryHandler _handler;

    public GetFinanceReportsQueryHandlerTests()
    {
        _repoMock = new Mock<IInvoiceRepository>();
        _handler = new GetFinanceReportsQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsReportDto_WithCalculatedValues()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 1000, TaxTotal = 180, DiscountTotal = 0, AmountDue = 0, Status = InvoiceStatus.Paid, InvoiceNumber = "INV-001", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 500, TaxTotal = 90, DiscountTotal = 50, AmountDue = 550, Status = InvoiceStatus.Issued, InvoiceNumber = "INV-002", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 200, TaxTotal = 0, DiscountTotal = 0, AmountDue = 200, Status = InvoiceStatus.PartiallyPaid, InvoiceNumber = "INV-003", Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);

        var result = await _handler.Handle(new GetFinanceReportsQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(1000);
        result.Value.NetIncome.Should().Be(1000);
        result.Value.TotalTaxCollected.Should().Be(180);
        result.Value.TotalDiscountGiven.Should().Be(0);
        result.Value.TotalInvoicesIssued.Should().Be(3);
        result.Value.TotalPaymentsReceived.Should().Be(1);
        result.Value.OutstandingReceivables.Should().Be(750);
    }

    [Fact]
    public async Task Handle_ReturnsZeroValues_WhenNoInvoices()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Invoice>());

        var result = await _handler.Handle(new GetFinanceReportsQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalRevenue.Should().Be(0);
        result.Value.TotalInvoicesIssued.Should().Be(0);
        result.Value.TotalPaymentsReceived.Should().Be(0);
        result.Value.OutstandingReceivables.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CalculatesOutstandingReceivables()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), Total = 1000, TaxTotal = 180, DiscountTotal = 0, AmountDue = 1000, Status = InvoiceStatus.Issued, InvoiceNumber = "INV-001", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 500, TaxTotal = 90, DiscountTotal = 0, AmountDue = 200, Status = InvoiceStatus.PartiallyPaid, InvoiceNumber = "INV-002", Currency = "INR" },
            new() { Id = Guid.NewGuid(), Total = 300, TaxTotal = 0, DiscountTotal = 0, AmountDue = 0, Status = InvoiceStatus.Paid, InvoiceNumber = "INV-003", Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);

        var result = await _handler.Handle(new GetFinanceReportsQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.OutstandingReceivables.Should().Be(1200);
    }
}

public class ValidateCouponQueryHandlerTests
{
    private readonly Mock<ICouponService> _serviceMock;
    private readonly ValidateCouponQueryHandler _handler;

    public ValidateCouponQueryHandlerTests()
    {
        _serviceMock = new Mock<ICouponService>();
        _handler = new ValidateCouponQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenCouponValid()
    {
        var query = new ValidateCouponQuery("SAVE10", "user-1", 1000);
        _serviceMock.Setup(s => s.ValidateCouponAsync("SAVE10", "user-1", 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenCouponInvalid()
    {
        var query = new ValidateCouponQuery("INVALID", "user-1", 100);
        _serviceMock.Setup(s => s.ValidateCouponAsync("INVALID", "user-1", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(false));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenServiceReturnsFailure()
    {
        var query = new ValidateCouponQuery("ERROR", "user-1", 100);
        _serviceMock.Setup(s => s.ValidateCouponAsync("ERROR", "user-1", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Service error"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Service error");
    }
}

public class GetAllCouponsQueryHandlerTests
{
    private readonly Mock<ICouponRepository> _repoMock;
    private readonly GetAllCouponsQueryHandler _handler;

    public GetAllCouponsQueryHandlerTests()
    {
        _repoMock = new Mock<ICouponRepository>();
        _handler = new GetAllCouponsQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCoupons()
    {
        var coupons = new List<Coupon>
        {
            new() { Id = Guid.NewGuid(), Code = "SAVE10", Type = DiscountType.Percentage, Value = 10, MinOrderAmount = 500, MaxDiscountAmount = 100, MaxUsage = 100, CurrentUsage = 5, ValidFrom = DateTime.UtcNow.AddDays(-30), ValidTo = DateTime.UtcNow.AddDays(30), IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "FLAT50", Type = DiscountType.Flat, Value = 50, MinOrderAmount = 200, MaxDiscountAmount = null, MaxUsage = 50, CurrentUsage = 0, ValidFrom = DateTime.UtcNow.AddDays(-10), ValidTo = DateTime.UtcNow.AddDays(10), IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(coupons);

        var result = await _handler.Handle(new GetAllCouponsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Code.Should().Be("SAVE10");
        result.Value[1].Code.Should().Be("FLAT50");
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoCoupons()
    {
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Coupon>());

        var result = await _handler.Handle(new GetAllCouponsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MapsCouponsToDtosCorrectly()
    {
        var couponId = Guid.NewGuid();
        var coupons = new List<Coupon>
        {
            new() { Id = couponId, Code = "TEST", Type = DiscountType.Percentage, Value = 15, MinOrderAmount = 300, MaxDiscountAmount = 75, MaxUsage = 10, CurrentUsage = 2, ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddMonths(1), IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(coupons);

        var result = await _handler.Handle(new GetAllCouponsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value[0];
        dto.Id.Should().Be(couponId);
        dto.Code.Should().Be("TEST");
        dto.Type.Should().Be(DiscountType.Percentage);
        dto.Value.Should().Be(15);
        dto.MinimumOrderAmount.Should().Be(300);
        dto.MaximumDiscountAmount.Should().Be(75);
        dto.MaxUsages.Should().Be(10);
        dto.CurrentUsages.Should().Be(2);
        dto.IsActive.Should().BeTrue();
    }
}

public class GetCouponByCodeQueryHandlerTests
{
    private readonly Mock<ICouponService> _serviceMock;
    private readonly GetCouponByCodeQueryHandler _handler;

    public GetCouponByCodeQueryHandlerTests()
    {
        _serviceMock = new Mock<ICouponService>();
        _handler = new GetCouponByCodeQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCouponDto_WhenFound()
    {
        var code = "SAVE10";
        var dto = new CouponDto(Guid.NewGuid(), code, "Save 10%", DiscountType.Percentage, 10, 500, 100, 100, 5, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30), true, DateTime.UtcNow);
        _serviceMock.Setup(s => s.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Success(dto));

        var result = await _handler.Handle(new GetCouponByCodeQuery(code), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNotFound()
    {
        var code = "NONEXISTENT";
        _serviceMock.Setup(s => s.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Coupon not found"));

        var result = await _handler.Handle(new GetCouponByCodeQuery(code), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coupon not found");
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectCode()
    {
        var code = "MYCOUPON";
        _serviceMock.Setup(s => s.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CouponDto>.Failure("Not found"));

        await _handler.Handle(new GetCouponByCodeQuery(code), CancellationToken.None);

        _serviceMock.Verify(s => s.GetByCodeAsync(code, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetAllScholarshipsQueryHandlerTests
{
    private readonly GetAllScholarshipsQueryHandler _handler;

    public GetAllScholarshipsQueryHandlerTests()
    {
        _handler = new GetAllScholarshipsQueryHandler();
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList()
    {
        var result = await _handler.Handle(new GetAllScholarshipsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResult()
    {
        var result = await _handler.Handle(new GetAllScholarshipsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeAssignableTo<IReadOnlyList<ScholarshipDto>>();
    }
}

public class PaymentSearchQueryHandlerTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly PaymentSearchQueryHandler _handler;

    public PaymentSearchQueryHandlerTests()
    {
        _repoMock = new Mock<IPaymentRepository>();
        _handler = new PaymentSearchQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_SearchesByInvoiceId_WhenProvided()
    {
        var invoiceId = Guid.NewGuid();
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 500, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetByInvoiceIdAsync(invoiceId, It.IsAny<CancellationToken>())).ReturnsAsync(payments);

        var query = new PaymentSearchQuery(null, null, invoiceId, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        _repoMock.Verify(r => r.GetByInvoiceIdAsync(invoiceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SearchesAll_WhenNoInvoiceId()
    {
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 500, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" },
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-002", Amount = 300, Status = PaymentStatus.Pending, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);

        var query = new PaymentSearchQuery(null, null, null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FiltersBySearchTermAndStatus()
    {
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 500, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" },
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-002", Amount = 300, Status = PaymentStatus.Failed, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" },
            new() { Id = Guid.NewGuid(), PaymentReference = "TXN-001", Amount = 200, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);

        var query = new PaymentSearchQuery("PAY", PaymentStatus.Captured, null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].PaymentReference.Should().Be("PAY-001");
    }

    [Fact]
    public async Task Handle_AppliesDateRangeFilter()
    {
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 500, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow.AddDays(-5), CreatedAt = DateTime.UtcNow, Currency = "INR" },
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-002", Amount = 300, Status = PaymentStatus.Captured, PaymentDate = DateTime.UtcNow.AddDays(-20), CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);

        var query = new PaymentSearchQuery(null, null, null, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_AppliesPagination()
    {
        var payments = Enumerable.Range(1, 10).Select(i => new Payment
        {
            Id = Guid.NewGuid(),
            PaymentReference = $"PAY-{i:D3}",
            Amount = i * 100,
            Status = PaymentStatus.Captured,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Currency = "INR"
        }).ToList();
        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);

        var query = new PaymentSearchQuery(null, null, null, null, null, 2, 3);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value[0].PaymentReference.Should().Be("PAY-004");
    }
}

public class FinanceSearchQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IRefundRepository> _refundRepoMock;
    private readonly FinanceSearchQueryHandler _handler;

    public FinanceSearchQueryHandlerTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _refundRepoMock = new Mock<IRefundRepository>();
        _handler = new FinanceSearchQueryHandler(_invoiceRepoMock.Object, _paymentRepoMock.Object, _refundRepoMock.Object);
    }

    [Fact]
    public async Task Handle_SearchesAllEntities_WithSearchTerm()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Total = 1000, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, Currency = "INR" },
            new() { Id = Guid.NewGuid(), InvoiceNumber = "OTHER", Total = 500, Status = InvoiceStatus.Issued, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 1000, Status = PaymentStatus.Captured, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        var refunds = new List<Refund>
        {
            new() { Id = Guid.NewGuid(), RefundNumber = "RFD-001", TotalAmount = 100, Status = RefundStatus.Completed, CreatedAt = DateTime.UtcNow, PaymentId = Guid.NewGuid() }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);
        _paymentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);
        _refundRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(refunds);

        var query = new FinanceSearchQuery("INV", null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Invoices.Should().HaveCount(1);
        result.Value.Payments.Should().HaveCount(0);
        result.Value.Refunds.Should().HaveCount(0);
        result.Value.TotalResults.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ReturnsAllResults_WhenNoSearchTerm()
    {
        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), InvoiceNumber = "INV-001", Total = 1000, Status = InvoiceStatus.Paid, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), PaymentReference = "PAY-001", Amount = 1000, Status = PaymentStatus.Captured, CreatedAt = DateTime.UtcNow, Currency = "INR" }
        };
        var refunds = new List<Refund>
        {
            new() { Id = Guid.NewGuid(), RefundNumber = "RFD-001", TotalAmount = 100, Status = RefundStatus.Completed, CreatedAt = DateTime.UtcNow, PaymentId = Guid.NewGuid() }
        };
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);
        _paymentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(payments);
        _refundRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(refunds);

        var query = new FinanceSearchQuery(null, null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Invoices.Should().HaveCount(1);
        result.Value.Payments.Should().HaveCount(1);
        result.Value.Refunds.Should().HaveCount(1);
        result.Value.TotalResults.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyHits_WhenNoMatch()
    {
        _invoiceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Invoice>());
        _paymentRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Payment>());
        _refundRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Refund>());

        var query = new FinanceSearchQuery("NONEXISTENT", null, null, null, 1, 20);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Invoices.Should().BeEmpty();
        result.Value.Payments.Should().BeEmpty();
        result.Value.Refunds.Should().BeEmpty();
        result.Value.TotalResults.Should().Be(0);
    }
}

public class GetInvoiceReceiptQueryHandlerTests
{
    private readonly Mock<IInvoiceRepository> _repoMock;
    private readonly GetInvoiceReceiptQueryHandler _handler;

    public GetInvoiceReceiptQueryHandlerTests()
    {
        _repoMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoiceReceiptQueryHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsReceiptDto_WhenInvoiceExists()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            InvoiceNumber = "INV-001",
            IssueDate = DateTime.UtcNow,
            SubTotal = 1000,
            TaxTotal = 180,
            DiscountTotal = 0,
            Total = 1180,
            AmountPaid = 1180,
            Currency = "INR",
            Items = new List<InvoiceItem>
            {
                new() { Description = "Coaching Fee", Quantity = 1, UnitPrice = 1000, TotalAmount = 1000 }
            }
        };
        _repoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var result = await _handler.Handle(new GetInvoiceReceiptQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(invoiceId);
        result.Value.InvoiceNumber.Should().Be("INV-001");
        result.Value.SubTotal.Should().Be(1000);
        result.Value.TaxAmount.Should().Be(180);
        result.Value.TotalAmount.Should().Be(1180);
        result.Value.AmountPaid.Should().Be(1180);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenInvoiceNotFound()
    {
        var invoiceId = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);

        var result = await _handler.Handle(new GetInvoiceReceiptQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task Handle_MapsLineItemsCorrectly()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            InvoiceNumber = "INV-002",
            IssueDate = DateTime.UtcNow,
            SubTotal = 1500,
            TaxTotal = 0,
            DiscountTotal = 0,
            Total = 1500,
            AmountPaid = 1500,
            Currency = "INR",
            Items = new List<InvoiceItem>
            {
                new() { Description = "Item 1", Quantity = 2, UnitPrice = 500, TotalAmount = 1000 },
                new() { Description = "Item 2", Quantity = 1, UnitPrice = 500, TotalAmount = 500 }
            }
        };
        _repoMock.Setup(r => r.GetByIdWithDetailsAsync(invoiceId, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

        var result = await _handler.Handle(new GetInvoiceReceiptQuery(invoiceId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LineItems.Should().HaveCount(2);
        result.Value.LineItems[0].Description.Should().Be("Item 1");
        result.Value.LineItems[0].Quantity.Should().Be(2);
        result.Value.LineItems[0].UnitPrice.Should().Be(500);
        result.Value.LineItems[0].Total.Should().Be(1000);
    }
}

public class GetWalletTransactionsQueryHandlerTests
{
    private readonly Mock<IWalletService> _serviceMock;
    private readonly GetWalletTransactionsQueryHandler _handler;

    public GetWalletTransactionsQueryHandlerTests()
    {
        _serviceMock = new Mock<IWalletService>();
        _handler = new GetWalletTransactionsQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTransactions()
    {
        var walletId = Guid.NewGuid();
        var transactions = new List<WalletTransactionDto>
        {
            new(Guid.NewGuid(), walletId, TransactionType.Credit, 1000, 5000, 6000, "REF-001", "Payment received", DateTime.UtcNow)
        };
        _serviceMock.Setup(s => s.GetTransactionsAsync(walletId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WalletTransactionDto>>.Success(transactions));

        var result = await _handler.Handle(new GetWalletTransactionsQuery(walletId, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoTransactions()
    {
        var walletId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetTransactionsAsync(walletId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WalletTransactionDto>>.Success(new List<WalletTransactionDto>()));

        var result = await _handler.Handle(new GetWalletTransactionsQuery(walletId, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceWithCorrectParameters()
    {
        var walletId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetTransactionsAsync(walletId, 2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WalletTransactionDto>>.Success(new List<WalletTransactionDto>()));

        await _handler.Handle(new GetWalletTransactionsQuery(walletId, 2, 10), CancellationToken.None);

        _serviceMock.Verify(s => s.GetTransactionsAsync(walletId, 2, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetPaymentStatisticsQueryHandlerTests
{
    private readonly GetPaymentStatisticsQueryHandler _handler;

    public GetPaymentStatisticsQueryHandlerTests()
    {
        _handler = new GetPaymentStatisticsQueryHandler();
    }

    [Fact]
    public async Task Handle_ReturnsDefaultStatistics()
    {
        var result = await _handler.Handle(new GetPaymentStatisticsQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalTransactions.Should().Be(0);
        result.Value.TotalAmount.Should().Be(0);
        result.Value.SuccessfulAmount.Should().Be(0);
        result.Value.FailedAmount.Should().Be(0);
        result.Value.RefundedAmount.Should().Be(0);
        result.Value.SuccessfulCount.Should().Be(0);
        result.Value.FailedCount.Should().Be(0);
        result.Value.RefundedCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResult()
    {
        var result = await _handler.Handle(new GetPaymentStatisticsQuery(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<PaymentStatisticsDto>();
    }
}
