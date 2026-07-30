using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Entities.Finance;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Infrastructure.Persistence;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class RefundWorkflowTests : FinanceTestBase
{
    public RefundWorkflowTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<(Invoice Invoice, Payment Payment)> CreatePaidInvoiceAndPaymentAsync()
    {
        var dbContext = ServiceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var now = DateTime.UtcNow;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INV-{Guid.NewGuid():N}"[..20],
            IssueDate = now,
            DueDate = now.AddDays(30),
            Status = InvoiceStatus.Paid,
            SubTotal = 1000m,
            TaxTotal = 180m,
            DiscountTotal = 0m,
            Total = 1180m,
            AmountPaid = 1180m,
            AmountDue = 0m,
            Currency = "INR",
        };
        dbContext.Invoices.Add(invoice);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentReference = $"PAY-{Guid.NewGuid():N}"[..20],
            PaymentDate = now,
            Amount = 1180m,
            Currency = "INR",
            PaymentMethod = Domain.Enums.Finance.PaymentMethod.Cash,
            Status = PaymentStatus.Captured,
            InvoiceId = invoice.Id,
        };
        dbContext.Payments.Add(payment);

        await dbContext.SaveChangesAsync();
        return (invoice, payment);
    }

    [Fact]
    public async Task RequestRefund_WithValidData_ReturnsSuccess()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var command = new RequestRefundCommand(payment.Id, 500m, "Customer requested refund", null);
        var result = await SendAsync<Result<RefundDto>>(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.PaymentId.Should().Be(payment.Id);
        result.Value.Amount.Should().Be(500m);
        result.Value.Status.Should().Be(RefundStatus.Requested);
    }

    [Fact]
    public async Task RequestRefund_WithInvalidReason_ReturnsFailure()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var command = new RequestRefundCommand(payment.Id, 500m, string.Empty, null);
        var result = await SendAsync<Result<RefundDto>>(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RequestRefund_ForNonExistentPayment_ReturnsFailure()
    {
        var command = new RequestRefundCommand(Guid.NewGuid(), 500m, "Refund reason", null);
        var result = await SendAsync<Result<RefundDto>>(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ApproveRefund_ChangesStatusToApproved()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var requestResult = await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(payment.Id, 500m, "Customer request", null));
        var refundId = requestResult.Value!.Id;

        var approveResult = await SendAsync<Result<RefundDto>>(
            new ApproveRefundCommand(refundId, "Admin"));

        approveResult.IsSuccess.Should().BeTrue();
        approveResult.Value!.Status.Should().Be(RefundStatus.Approved);
    }

    [Fact]
    public async Task RejectRefund_ChangesStatusToRejected()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var requestResult = await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(payment.Id, 500m, "Customer request", null));
        var refundId = requestResult.Value!.Id;

        var rejectResult = await SendAsync<Result<RefundDto>>(
            new RejectRefundCommand(refundId, "Policy does not allow refunds"));

        rejectResult.IsSuccess.Should().BeTrue();
        rejectResult.Value!.Status.Should().Be(RefundStatus.Rejected);
    }

    [Fact]
    public async Task CompleteRefund_WithApprovedRefund_ReturnsSuccess()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var requestResult = await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(payment.Id, 500m, "Customer request", null));
        var refundId = requestResult.Value!.Id;

        await SendAsync<Result<RefundDto>>(new ApproveRefundCommand(refundId, "Admin"));

        var completeResult = await SendAsync<Result<RefundDto>>(
            new CompleteRefundCommand(refundId, "GATEWAY-REF-001"));

        completeResult.IsSuccess.Should().BeTrue();
        completeResult.Value!.Status.Should().Be(RefundStatus.Completed);
    }

    [Fact]
    public async Task GetRefundHistory_ReturnsRefunds()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(payment.Id, 500m, "Customer request", null));

        var historyResult = await SendAsync<Result<IReadOnlyList<RefundDto>>>(
            new GetRefundHistoryQuery(payment.Id));

        historyResult.IsSuccess.Should().BeTrue();
        historyResult.Value.Should().NotBeEmpty();
        historyResult.Value.Should().Contain(r => r.PaymentId == payment.Id);
    }

    [Fact]
    public async Task Refund_UpdatesLedgerBalance()
    {
        var (_, payment) = await CreatePaidInvoiceAndPaymentAsync();

        var requestResult = await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(payment.Id, 500m, "Customer request", null));
        var refundId = requestResult.Value!.Id;

        await SendAsync<Result<RefundDto>>(new ApproveRefundCommand(refundId, "Admin"));
        await SendAsync<Result<RefundDto>>(new CompleteRefundCommand(refundId, "GATEWAY-002"));

        var dbContext = ServiceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var ledgerEntries = await dbContext.LedgerEntries
            .Where(le => le.Reference == refundId.ToString())
            .ToListAsync();

        ledgerEntries.Should().NotBeEmpty();
        ledgerEntries.Should().Contain(le => le.DebitAmount > 0 || le.CreditAmount > 0);
    }
}
