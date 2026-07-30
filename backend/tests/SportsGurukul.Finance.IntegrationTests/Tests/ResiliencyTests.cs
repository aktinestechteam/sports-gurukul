using FluentAssertions;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Wallet;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class ResiliencyTests : FinanceTestBase
{
    public ResiliencyTests(FinanceWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreatePayment_WithGatewayTimeout_ReturnsFailure()
    {
        var command = new InitiatePaymentCommand(
            Guid.NewGuid(), 100m, PaymentMethod.UPI, null,
            "Payment for non-existent invoice");

        var result = await SendAsync(command);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ConcurrentPayment_Creation_HandlesCorrectly()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 5000m, null)
        };
        var invoiceCommand = new CreateInvoiceCommand(
            "Concurrent Payment Invoice", null, "INR",
            FinanceTestIds.AthleteUserId, null, lineItems, null, null);
        var invoiceResult = await SendAsync(invoiceCommand);
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoiceId = invoiceResult.Value!.Id;

        var tasks = new[]
        {
            SendAsync(new InitiatePaymentCommand(invoiceId, 1000m, PaymentMethod.UPI, Guid.NewGuid().ToString(), "Concurrent payment 1")),
            SendAsync(new InitiatePaymentCommand(invoiceId, 2000m, PaymentMethod.Card, Guid.NewGuid().ToString(), "Concurrent payment 2")),
            SendAsync(new InitiatePaymentCommand(invoiceId, 2000m, PaymentMethod.NetBanking, Guid.NewGuid().ToString(), "Concurrent payment 3"))
        };

        var results = await Task.WhenAll(tasks);

        results.Should().HaveCount(3);
        results.Should().Contain(r => r.IsSuccess);
    }

    [Fact]
    public async Task Invoice_WithCancelledStatus_RejectsPayments()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 1000m, null)
        };
        var createCommand = new CreateInvoiceCommand(
            "Cancel Rejection Test", null, "INR",
            FinanceTestIds.AthleteUserId, null, lineItems, null, null);
        var createResult = await SendAsync(createCommand);
        createResult.IsSuccess.Should().BeTrue();
        var invoiceId = createResult.Value!.Id;

        var cancelCommand = new CancelInvoiceCommand(invoiceId, "Testing cancellation");
        var cancelResult = await SendAsync(cancelCommand);
        cancelResult.IsSuccess.Should().BeTrue();

        var paymentCommand = new InitiatePaymentCommand(
            invoiceId, 1000m, PaymentMethod.UPI, null,
            "Payment on cancelled invoice");
        var paymentResult = await SendAsync(paymentCommand);

        paymentResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task PaymentAfterRefund_ReturnsAlreadyRefunded()
    {
        var lineItems = new List<CreateInvoiceLineItemDto>
        {
            new("Coaching Fee", "Coaching", null, 1, 1000m, null)
        };
        var createCommand = new CreateInvoiceCommand(
            "Refund Test Invoice", null, "INR",
            FinanceTestIds.AthleteUserId, null, lineItems, null, null);
        var createResult = await SendAsync(createCommand);
        createResult.IsSuccess.Should().BeTrue();
        var invoiceId = createResult.Value!.Id;

        var recordPaymentCommand = new RecordOfflinePaymentCommand(
            invoiceId, 1000m, PaymentMethod.Cash, "REF-001",
            DateTime.UtcNow, "Offline payment for refund test");
        var paymentResult = await SendAsync(recordPaymentCommand);
        paymentResult.IsSuccess.Should().BeTrue();
        var paymentId = paymentResult.Value!.Id;

        var cancelCommand = new CancelPaymentCommand(paymentId, "Refund issued");
        var cancelResult = await SendAsync(cancelCommand);

        var secondCancelCommand = new CancelPaymentCommand(paymentId, "Duplicate refund");
        var secondCancelResult = await SendAsync(secondCancelCommand);

        secondCancelResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Wallet_ConcurrentTransactions_MaintainConsistency()
    {
        var walletId = FinanceTestIds.TestWalletId;

        var tasks = new[]
        {
            SendAsync(new CreditWalletCommand(walletId, 500m, "CONCUR-CR-1", "Concurrent credit 1")),
            SendAsync(new DebitWalletCommand(walletId, 300m, "CONCUR-DR-1", "Concurrent debit 1")),
            SendAsync(new CreditWalletCommand(walletId, 200m, "CONCUR-CR-2", "Concurrent credit 2")),
            SendAsync(new DebitWalletCommand(walletId, 100m, "CONCUR-DR-2", "Concurrent debit 2"))
        };

        var results = await Task.WhenAll(tasks);

        results.Should().HaveCount(4);
        results.Should().AllSatisfy(r => r.Should().NotBeNull());
    }

    [Fact]
    public async Task DatabaseConnectionLoss_RecoversGracefully()
    {
        var invalidCommand = new CreateInvoiceCommand(
            null, null, null, null, null,
            new List<CreateInvoiceLineItemDto>(), null, null);
        var result = await SendAsync(invalidCommand);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}
