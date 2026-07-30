using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Invoice;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Payment;
using SportsGurukul.Application.Features.FinanceManagement.Commands.Refund;
using SportsGurukul.Application.Features.FinanceManagement.DTOs;
using SportsGurukul.Application.Features.FinanceManagement.Queries;
using SportsGurukul.Domain.Enums.Finance;
using SportsGurukul.Finance.IntegrationTests.Fixtures;
using SportsGurukul.Finance.IntegrationTests.Helpers;
using SportsGurukul.Finance.IntegrationTests.Seed;
using SportsGurukul.Infrastructure.Persistence;
using SportsGurukul.Platform.PaymentGateway.Models;
using Xunit;

namespace SportsGurukul.Finance.IntegrationTests.Tests;

[Collection("Finance")]
public class CrossModuleTests : FinanceTestBase
{
    private const string OrdersUrl = "api/v1/payments/orders";

    public CrossModuleTests(FinanceWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatePayment_WithInvoice_UpdatesBothEntities()
    {
        var createInvoice = new CreateInvoiceCommand(
            "Test invoice for payment",
            DateTime.UtcNow.AddDays(30),
            "INR",
            FinanceTestIds.AthleteUserId,
            null,
            [new CreateInvoiceLineItemDto("Coaching Fee", "Service", null, 1, 5000m, null)],
            null,
            null);

        var invoiceResult = await SendAsync<Result<InvoiceDto>>(createInvoice);
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoice = invoiceResult.Value!;
        invoice.Status.Should().Be(InvoiceStatus.Draft);

        var issueResult = await SendAsync<Result<InvoiceDto>>(new IssueInvoiceCommand(invoice.Id));
        issueResult.IsSuccess.Should().BeTrue();
        issueResult.Value!.Status.Should().Be(InvoiceStatus.Issued);

        var paymentResult = await SendAsync<Result<PaymentDto>>(
            new RecordOfflinePaymentCommand(
                invoice.Id,
                invoice.TotalAmount,
                Domain.Enums.Finance.PaymentMethod.Cash,
                "CHQ-001",
                DateTime.UtcNow,
                "Offline payment"));

        paymentResult.IsSuccess.Should().BeTrue();
        paymentResult.Value!.InvoiceId.Should().Be(invoice.Id);
        paymentResult.Value.Amount.Should().Be(invoice.TotalAmount);
        paymentResult.Value.Status.Should().Be(PaymentStatus.Captured);

        var updatedInvoice = await SendAsync<Result<InvoiceDto>>(new GetInvoiceByIdQuery(invoice.Id));
        updatedInvoice.IsSuccess.Should().BeTrue();
        updatedInvoice.Value!.Status.Should().Be(InvoiceStatus.Paid);
        updatedInvoice.Value.PaidAmount.Should().Be(invoice.TotalAmount);
    }

    [Fact]
    public async Task PaymentGateway_CreateOrder_ThenCheckStatus()
    {
        var orderRequest = new
        {
            orderId = $"test_{Guid.NewGuid():N}",
            amount = 50000,
            currency = "INR",
            description = "Integration test order"
        };

        var createResponse = await PostAsJsonAsync(AdminClient, OrdersUrl, orderRequest);
        createResponse.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.Created,
            System.Net.HttpStatusCode.OK);

        var createdOrder = await createResponse.Content.ReadFromJsonAsync<PaymentOrderResponse>(JsonOptions);
        createdOrder.Should().NotBeNull();
        createdOrder!.GatewayOrderId.Should().NotBeNullOrEmpty();

        var statusResponse = await GetAsync(AdminClient, $"{OrdersUrl}/{createdOrder.GatewayOrderId}/status");
        statusResponse.StatusCode.Should().BeOneOf(
            System.Net.HttpStatusCode.OK,
            System.Net.HttpStatusCode.NotFound);

        if (statusResponse.IsSuccessStatusCode)
        {
            var status = await statusResponse.Content.ReadFromJsonAsync<PaymentStatusResponse>(JsonOptions);
            status.Should().NotBeNull();
            status!.GatewayOrderId.Should().Be(createdOrder.GatewayOrderId);
        }
    }

    [Fact]
    public async Task Payment_Refund_Ledger_Consistency()
    {
        var createInvoice = new CreateInvoiceCommand(
            "Invoice for refund test",
            DateTime.UtcNow.AddDays(30),
            "INR",
            FinanceTestIds.AthleteUserId,
            null,
            [new CreateInvoiceLineItemDto("Training Fee", "Service", null, 1, 10000m, null)],
            null,
            null);

        var invoiceResult = await SendAsync<Result<InvoiceDto>>(createInvoice);
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoice = invoiceResult.Value!;

        await SendAsync<Result<InvoiceDto>>(new IssueInvoiceCommand(invoice.Id));

        var paymentResult = await SendAsync<Result<PaymentDto>>(
            new RecordOfflinePaymentCommand(
                invoice.Id,
                invoice.TotalAmount,
                Domain.Enums.Finance.PaymentMethod.BankTransfer,
                "BT-REF-001",
                DateTime.UtcNow,
                "Bank transfer payment"));

        paymentResult.IsSuccess.Should().BeTrue();
        var payment = paymentResult.Value!;

        var refundResult = await SendAsync<Result<RefundDto>>(
            new RequestRefundCommand(
                payment.Id,
                payment.Amount,
                "Customer requested refund",
                null));

        refundResult.IsSuccess.Should().BeTrue();
        var refund = refundResult.Value!;
        refund.Status.Should().Be(RefundStatus.Requested);

        var approveResult = await SendAsync<Result<RefundDto>>(
            new ApproveRefundCommand(refund.Id, "System"));
        approveResult.IsSuccess.Should().BeTrue();
        approveResult.Value!.Status.Should().Be(RefundStatus.Approved);

        var completeResult = await SendAsync<Result<RefundDto>>(
            new CompleteRefundCommand(refund.Id, "GATEWAY-REF-001"));
        completeResult.IsSuccess.Should().BeTrue();
        completeResult.Value!.Status.Should().Be(RefundStatus.Completed);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var ledger = await dbContext.Ledgers.FindAsync(FinanceTestIds.TestLedgerId);
        ledger.Should().NotBeNull();
        ledger!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Wallet_Credit_Then_UseForPayment()
    {
        var creditResult = await SendAsync<Result<WalletDto>>(
            new Application.Features.FinanceManagement.Commands.Wallet.CreditWalletCommand(
                FinanceTestIds.TestWalletId,
                5000m,
                "REF-CREDIT-001",
                "Test wallet credit"));

        creditResult.IsSuccess.Should().BeTrue();
        var creditedWallet = creditResult.Value!;
        creditedWallet.Balance.Should().Be(15000m);

        var debitResult = await SendAsync<Result<WalletDto>>(
            new Application.Features.FinanceManagement.Commands.Wallet.DebitWalletCommand(
                FinanceTestIds.TestWalletId,
                3000m,
                "REF-DEBIT-001",
                "Payment for services"));

        debitResult.IsSuccess.Should().BeTrue();
        var debitedWallet = debitResult.Value!;
        debitedWallet.Balance.Should().Be(12000m);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var wallet = await dbContext.Wallets.FindAsync(FinanceTestIds.TestWalletId);
        wallet.Should().NotBeNull();
        wallet!.Balance.Should().Be(12000m);
    }

    [Fact]
    public async Task Coupon_Discount_AppliedToInvoice()
    {
        var createInvoice = new CreateInvoiceCommand(
            "Invoice with coupon discount",
            DateTime.UtcNow.AddDays(30),
            "INR",
            FinanceTestIds.AthleteUserId,
            null,
            [new CreateInvoiceLineItemDto("Equipment Fee", "Service", null, 2, 3000m, null)],
            "WELCOME10",
            null);

        var invoiceResult = await SendAsync<Result<InvoiceDto>>(createInvoice);
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoice = invoiceResult.Value!;

        invoice.DiscountAmount.Should().BeGreaterThan(0);
        invoice.SubTotal.Should().Be(6000m);
        invoice.DiscountAmount.Should().Be(500m);
        invoice.TotalAmount.Should().Be(invoice.SubTotal - invoice.DiscountAmount + invoice.TaxAmount);
    }

    [Fact]
    public async Task Invoice_To_Receipt_Workflow()
    {
        var createInvoice = new CreateInvoiceCommand(
            "Receipt workflow test",
            DateTime.UtcNow.AddDays(15),
            "INR",
            FinanceTestIds.AthleteUserId,
            null,
            [new CreateInvoiceLineItemDto("Membership Fee", "Service", null, 1, 2500m, null)],
            null,
            null);

        var invoiceResult = await SendAsync<Result<InvoiceDto>>(createInvoice);
        invoiceResult.IsSuccess.Should().BeTrue();
        var invoice = invoiceResult.Value!;

        await SendAsync<Result<InvoiceDto>>(new IssueInvoiceCommand(invoice.Id));

        var markPaidResult = await SendAsync<Result<InvoiceDto>>(
            new MarkInvoiceAsPaidCommand(invoice.Id));
        markPaidResult.IsSuccess.Should().BeTrue();
        markPaidResult.Value!.Status.Should().Be(InvoiceStatus.Paid);
        markPaidResult.Value.PaidAmount.Should().Be(invoice.TotalAmount);

        var receiptResult = await SendAsync<Result<InvoiceReceiptDto>>(
            new GetInvoiceReceiptQuery(invoice.Id));
        receiptResult.IsSuccess.Should().BeTrue();
        var receipt = receiptResult.Value!;

        receipt.Id.Should().Be(invoice.Id);
        receipt.InvoiceNumber.Should().NotBeNullOrEmpty();
        receipt.TotalAmount.Should().Be(invoice.TotalAmount);
        receipt.AmountPaid.Should().Be(invoice.TotalAmount);
        receipt.LineItems.Should().NotBeEmpty();
        receipt.LineItems.Should().Contain(li => li.Description == "Membership Fee");
    }
}
