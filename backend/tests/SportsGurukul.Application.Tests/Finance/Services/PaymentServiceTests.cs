using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.Finance;
using SportsGurukul.Application.Common.Interfaces.Finance.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Services;
using SportsGurukul.Domain.Entities.Finance;
using Fin = SportsGurukul.Domain.Enums.Finance;

namespace SportsGurukul.Application.Tests.Finance.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<ILedgerService> _ledgerServiceMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _ledgerServiceMock = new Mock<ILedgerService>();
        _service = new PaymentService(
            _paymentRepoMock.Object,
            _invoiceRepoMock.Object,
            _ledgerServiceMock.Object);
    }

    #region InitiatePaymentAsync

    [Fact]
    public async Task InitiatePaymentAsync_ValidRequest_ReturnsPendingPayment()
    {
        var invoiceId = Guid.NewGuid();
        var request = new InitiatePaymentRequest(invoiceId, 1000m, Fin.PaymentMethod.UPI, null, "Test payment");

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = invoiceId, Status = Fin.InvoiceStatus.Issued });
        _paymentRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);

        var result = await _service.InitiatePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Pending);
        result.Value.Amount.Should().Be(1000m);
        _paymentRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitiatePaymentAsync_InvoiceNotFound_ReturnsFailure()
    {
        var request = new InitiatePaymentRequest(Guid.NewGuid(), 500m, Fin.PaymentMethod.Card, null, null);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.InitiatePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    [Fact]
    public async Task InitiatePaymentAsync_WithIdempotencyKey_ReturnsExistingPayment()
    {
        var invoiceId = Guid.NewGuid();
        var request = new InitiatePaymentRequest(invoiceId, 500m, Fin.PaymentMethod.UPI, "idem-001", null);
        var existingPayment = new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            Amount = 500m,
            Status = Fin.PaymentStatus.Pending,
            PaymentReference = "PAY-001",
            PaymentMethod = Fin.PaymentMethod.UPI,
            IdempotencyKey = "idem-001"
        };

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = invoiceId });
        _paymentRepoMock.Setup(r => r.GetByIdempotencyKeyAsync("idem-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPayment);

        var result = await _service.InitiatePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _paymentRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitiatePaymentAsync_NewIdempotencyKey_CreatesNewPayment()
    {
        var invoiceId = Guid.NewGuid();
        var request = new InitiatePaymentRequest(invoiceId, 500m, Fin.PaymentMethod.UPI, "new-idem", null);

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = invoiceId });
        _paymentRepoMock.Setup(r => r.GetByIdempotencyKeyAsync("new-idem", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);
        _paymentRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);

        var result = await _service.InitiatePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _paymentRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region AuthorizePaymentAsync

    [Fact]
    public async Task AuthorizePaymentAsync_ValidPayment_ReturnsAuthorized()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment { Id = paymentId, Status = Fin.PaymentStatus.Pending, PaymentMethod = Fin.PaymentMethod.Card };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.AuthorizePaymentAsync(paymentId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Authorized);
        _paymentRepoMock.Verify(r => r.Update(payment), Times.Once);
    }

    [Fact]
    public async Task AuthorizePaymentAsync_PaymentNotFound_ReturnsFailure()
    {
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.AuthorizePaymentAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
    }

    [Fact]
    public async Task AuthorizePaymentAsync_NotPending_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Status = Fin.PaymentStatus.Captured };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.AuthorizePaymentAsync(payment.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only pending payments can be authorized");
    }

    #endregion

    #region CapturePaymentAsync

    [Fact]
    public async Task CapturePaymentAsync_ValidPayment_ReturnsCaptured()
    {
        var paymentId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = paymentId,
            Status = Fin.PaymentStatus.Authorized,
            Amount = 500m,
            InvoiceId = invoiceId,
            PaymentReference = "PAY-001",
            PaymentMethod = Fin.PaymentMethod.UPI
        };

        _paymentRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = invoiceId, Total = 1000m, AmountPaid = 0m, AmountDue = 1000m, Status = Fin.InvoiceStatus.Issued });

        var cashLedger = new Ledger { Id = Guid.NewGuid(), Code = "CASH" };
        var arLedger = new Ledger { Id = Guid.NewGuid(), Code = "AR" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("CASH", "Cash", Fin.LedgerType.Asset, "Cash & Bank", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(cashLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("AR", "Accounts Receivable", Fin.LedgerType.Asset, "Accounts Receivable", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(arLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.CapturePaymentAsync(paymentId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Captured);
        _paymentRepoMock.Verify(r => r.Update(payment), Times.Once);
    }

    [Fact]
    public async Task CapturePaymentAsync_PaymentNotFound_ReturnsFailure()
    {
        _paymentRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.CapturePaymentAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
    }

    [Fact]
    public async Task CapturePaymentAsync_NotAuthorized_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Status = Fin.PaymentStatus.Pending };
        _paymentRepoMock.Setup(r => r.GetByIdWithTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.CapturePaymentAsync(payment.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only authorized payments can be captured");
    }

    #endregion

    #region RecordOfflinePaymentAsync

    [Fact]
    public async Task RecordOfflinePaymentAsync_ValidRequest_ReturnsCapturedPayment()
    {
        var invoiceId = Guid.NewGuid();
        var request = new RecordOfflinePaymentRequest(invoiceId, 1000m, Fin.PaymentMethod.Cash, "CHQ-001", DateTime.UtcNow, "Cash payment");

        _invoiceRepoMock.Setup(r => r.GetByIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice { Id = invoiceId, Total = 1000m, AmountPaid = 0m, AmountDue = 1000m, Status = Fin.InvoiceStatus.Issued });
        _paymentRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment p, CancellationToken _) => p);

        var cashLedger = new Ledger { Id = Guid.NewGuid(), Code = "CASH" };
        var arLedger = new Ledger { Id = Guid.NewGuid(), Code = "AR" };
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("CASH", "Cash", Fin.LedgerType.Asset, "Cash & Bank", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(cashLedger));
        _ledgerServiceMock.Setup(l => l.GetOrCreateLedgerAsync("AR", "Accounts Receivable", Fin.LedgerType.Asset, "Accounts Receivable", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Ledger>.Success(arLedger));
        _ledgerServiceMock.Setup(l => l.PostLedgerEntryAsync(It.IsAny<Ledger>(), It.IsAny<LedgerEntry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _service.RecordOfflinePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Captured);
        _invoiceRepoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public async Task RecordOfflinePaymentAsync_InvoiceNotFound_ReturnsFailure()
    {
        var request = new RecordOfflinePaymentRequest(Guid.NewGuid(), 500m, Fin.PaymentMethod.Cash, null, DateTime.UtcNow, null);
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.RecordOfflinePaymentAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invoice not found");
    }

    #endregion

    #region CancelPaymentAsync

    [Fact]
    public async Task CancelPaymentAsync_NonCapturedPayment_ReturnsFailed()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment { Id = paymentId, Status = Fin.PaymentStatus.Pending };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.CancelPaymentAsync(paymentId, "Customer cancelled", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Failed);
        _paymentRepoMock.Verify(r => r.Update(payment), Times.Once);
    }

    [Fact]
    public async Task CancelPaymentAsync_CapturedPayment_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Status = Fin.PaymentStatus.Captured };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.CancelPaymentAsync(payment.Id, "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a completed payment");
    }

    [Fact]
    public async Task CancelPaymentAsync_PaymentNotFound_ReturnsFailure()
    {
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.CancelPaymentAsync(Guid.NewGuid(), "Reason", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
    }

    #endregion

    #region RetryPaymentAsync

    [Fact]
    public async Task RetryPaymentAsync_FailedPayment_ReturnsPending()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment { Id = paymentId, Status = Fin.PaymentStatus.Failed, FailureReason = "Timeout" };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.RetryPaymentAsync(paymentId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Fin.PaymentStatus.Pending);
        _paymentRepoMock.Verify(r => r.Update(payment), Times.Once);
    }

    [Fact]
    public async Task RetryPaymentAsync_NotFailed_ReturnsFailure()
    {
        var payment = new Payment { Id = Guid.NewGuid(), Status = Fin.PaymentStatus.Pending };
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _service.RetryPaymentAsync(payment.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only failed payments can be retried");
    }

    [Fact]
    public async Task RetryPaymentAsync_PaymentNotFound_ReturnsFailure()
    {
        _paymentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await _service.RetryPaymentAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Payment not found");
    }

    #endregion

    #region GeneratePaymentReferenceAsync

    [Fact]
    public async Task GeneratePaymentReferenceAsync_ReturnsFormattedReference()
    {
        _paymentRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(25);

        var result = await _service.GeneratePaymentReferenceAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Match("PAY-20260730-00026");
    }

    #endregion

    #region GetPaymentHistoryAsync

    [Fact]
    public async Task GetPaymentHistoryAsync_ReturnsPaymentsForInvoice()
    {
        var invoiceId = Guid.NewGuid();
        var payments = new List<Payment>
        {
            new() { Id = Guid.NewGuid(), InvoiceId = invoiceId, Amount = 500m, Status = Fin.PaymentStatus.Captured, PaymentMethod = Fin.PaymentMethod.UPI, PaymentReference = "PAY-001" }
        };
        _paymentRepoMock.Setup(r => r.GetByInvoiceIdAsync(invoiceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        var result = await _service.GetPaymentHistoryAsync(invoiceId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaymentHistoryAsync_NoPayments_ReturnsEmptyList()
    {
        _paymentRepoMock.Setup(r => r.GetByInvoiceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>());

        var result = await _service.GetPaymentHistoryAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    #endregion
}
