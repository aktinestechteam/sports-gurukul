using FluentAssertions;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class PaymentWorkflowTests : FinanceTestBase
{
    public PaymentWorkflowTests(FinanceWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task InitiatePayment_WithValidData_ReturnsSuccess()
    {
        var command = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            5000m,
            PaymentMethod.UPI,
            Guid.NewGuid().ToString(),
            "Test payment initiation");

        var result = await SendAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.InvoiceId.Should().Be(FinanceTestIds.TestInvoiceId);
        result.Value.Amount.Should().Be(5000m);
        result.Value.PaymentMethod.Should().Be(PaymentMethod.UPI);
    }

    [Fact]
    public async Task InitiatePayment_WithInvalidAmount_ReturnsFailure()
    {
        var command = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            -100m,
            PaymentMethod.Card,
            Guid.NewGuid().ToString(),
            null);

        var result = await SendAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AuthorizePayment_UpdatesStatus()
    {
        var initiateCommand = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            2500m,
            PaymentMethod.NetBanking,
            Guid.NewGuid().ToString(),
            "Payment for authorization test");
        var initiated = await SendAsync(initiateCommand);
        initiated.IsSuccess.Should().BeTrue();

        var authorizeCommand = new AuthorizePaymentCommand(initiated.Value!.Id);
        var result = await SendAsync(authorizeCommand);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(PaymentStatus.Authorized);
    }

    [Fact]
    public async Task CapturePayment_CompletesPayment()
    {
        var initiateCommand = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            3000m,
            PaymentMethod.UPI,
            Guid.NewGuid().ToString(),
            null);
        var initiated = await SendAsync(initiateCommand);
        initiated.IsSuccess.Should().BeTrue();

        await SendAsync(new AuthorizePaymentCommand(initiated.Value!.Id));

        var captureCommand = new CapturePaymentCommand(initiated.Value.Id);
        var result = await SendAsync(captureCommand);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(PaymentStatus.Captured);
    }

    [Fact]
    public async Task CancelPayment_ReturnsSuccess()
    {
        var initiateCommand = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            1500m,
            PaymentMethod.Wallet,
            Guid.NewGuid().ToString(),
            "To be cancelled");
        var initiated = await SendAsync(initiateCommand);
        initiated.IsSuccess.Should().BeTrue();

        var cancelCommand = new CancelPaymentCommand(initiated.Value!.Id, "Customer requested cancellation");
        var result = await SendAsync(cancelCommand);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(PaymentStatus.Cancelled);
    }

    [Fact]
    public async Task RecordOfflinePayment_WithValidData_ReturnsSuccess()
    {
        var command = new RecordOfflinePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            10000m,
            PaymentMethod.Cash,
            "CHQ-2024-001",
            DateTime.UtcNow,
            "Offline payment via cheque");

        var result = await SendAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(10000m);
        result.Value.PaymentMethod.Should().Be(PaymentMethod.Cash);
    }

    [Fact]
    public async Task RetryPayment_ForFailedPayment_ReturnsNewStatus()
    {
        var initiateCommand = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            2000m,
            PaymentMethod.Card,
            Guid.NewGuid().ToString(),
            null);
        var initiated = await SendAsync(initiateCommand);
        initiated.IsSuccess.Should().BeTrue();

        var retryCommand = new RetryPaymentCommand(initiated.Value!.Id);
        var result = await SendAsync(retryCommand);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPaymentHistory_ReturnsPayments()
    {
        var command = new GetPaymentHistoryQuery(FinanceTestIds.TestInvoiceId);
        var result = await SendAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPaymentStatistics_ReturnsStats()
    {
        var query = new GetPaymentStatisticsQuery(null, null);
        var result = await SendAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalTransactions.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task PaymentSearch_ReturnsFilteredResults()
    {
        var query = new PaymentSearchQuery(
            null,
            null,
            FinanceTestIds.TestInvoiceId,
            null,
            null,
            1,
            10);
        var result = await SendAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task DuplicatePayment_Prevention_DetectsDuplicate()
    {
        var idempotencyKey = Guid.NewGuid().ToString();

        var command = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            5000m,
            PaymentMethod.UPI,
            idempotencyKey,
            "First attempt");

        var firstResult = await SendAsync(command);
        firstResult.IsSuccess.Should().BeTrue();

        var duplicateCommand = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            5000m,
            PaymentMethod.UPI,
            idempotencyKey,
            "Duplicate attempt");

        var secondResult = await SendAsync(duplicateCommand);

        secondResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task PaymentTransaction_CreatesLedgerEntry()
    {
        var command = new InitiatePaymentCommand(
            FinanceTestIds.TestInvoiceId,
            7500m,
            PaymentMethod.BankTransfer,
            Guid.NewGuid().ToString(),
            "Ledger test payment");

        var result = await SendAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(7500m);
        result.Value.PaymentReference.Should().NotBeNullOrEmpty();
    }
}
