using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Adapters;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class GatewayFailureHandlingTests
{
    private readonly IPaymentSignatureValidator _validator;

    public GatewayFailureHandlingTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
    }

    [Fact]
    public async Task Razorpay_CreateOrder_WithNullOrderId_ReturnsFailed()
    {
        var adapter = new RazorpayGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
            NullLogger<RazorpayGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = null!, Amount = 100, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task Razorpay_CreateOrder_WithEmptyOrderId_ReturnsFailed()
    {
        var adapter = new RazorpayGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
            NullLogger<RazorpayGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "", Amount = 100, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task Stripe_CreateOrder_WithZeroAmount_ReturnsFailed()
    {
        var adapter = new StripeGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Stripe", UseSandbox = true },
            NullLogger<StripeGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "order_001", Amount = 0, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task Stripe_CreateOrder_WithNegativeAmount_ReturnsFailed()
    {
        var adapter = new StripeGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Stripe", UseSandbox = true },
            NullLogger<StripeGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "order_001", Amount = -100, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task Cashfree_Refund_WithNonExistentPayment_ReturnsFailed()
    {
        var adapter = new CashfreeGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
            NullLogger<CashfreeGateway>.Instance,
            _validator);

        var request = new PaymentRefundRequest { GatewayPaymentId = "non_existent_pay_123", Amount = 500 };

        var result = await adapter.RefundPaymentAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task Cashfree_Refund_WithEmptyPaymentId_ReturnsFailed()
    {
        var adapter = new CashfreeGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
            NullLogger<CashfreeGateway>.Instance,
            _validator);

        var request = new PaymentRefundRequest { GatewayPaymentId = "", Amount = 500 };

        var result = await adapter.RefundPaymentAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task PayU_CreateOrder_WithNullOrderId_ReturnsCreated()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = null!, Amount = 1000, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("created");
        result.GatewayOrderId.Should().BeNull();
    }

    [Fact]
    public async Task PayU_CreateOrder_WithEmptyOrderId_ReturnsCreated()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "", Amount = 1000, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("created");
        result.GatewayOrderId.Should().BeEmpty();
    }

    [Fact]
    public async Task PayPal_CreateOrder_InSandboxMode_ReturnsFailed()
    {
        var adapter = new PayPalGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "PayPal", UseSandbox = true },
            NullLogger<PayPalGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "order_sandbox", Amount = 100, Currency = "USD" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task AllAdapters_CreateOrder_ReturnsFailedForHttpBasedGateways()
    {
        var adapters = new IPaymentGateway[]
        {
            new RazorpayGateway(new HttpClient(), new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
                NullLogger<RazorpayGateway>.Instance, _validator),
            new StripeGateway(new HttpClient(), new GatewayConfig { Provider = "Stripe", UseSandbox = true },
                NullLogger<StripeGateway>.Instance, _validator),
            new CashfreeGateway(new HttpClient(), new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
                NullLogger<CashfreeGateway>.Instance, _validator),
            new PayPalGateway(new HttpClient(), new GatewayConfig { Provider = "PayPal", UseSandbox = true },
                NullLogger<PayPalGateway>.Instance, _validator)
        };

        var request = new PaymentOrderRequest { OrderId = "order_fail_test", Amount = 100, Currency = "INR" };

        foreach (var adapter in adapters)
        {
            var result = await adapter.CreateOrderAsync(request);
            result.Status.Should().Be("failed", $"{adapter.ProviderName} should fail gracefully");
        }
    }

    [Fact]
    public async Task PayU_CreateOrder_AlwaysSucceeds()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "order_payu_success", Amount = 500, Currency = "INR" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("created");
        result.Amount.Should().Be(500);
    }

    [Fact]
    public async Task AllAdapters_Refund_ReturnsFailedForHttpBasedGateways()
    {
        var adapters = new IPaymentGateway[]
        {
            new RazorpayGateway(new HttpClient(), new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
                NullLogger<RazorpayGateway>.Instance, _validator),
            new StripeGateway(new HttpClient(), new GatewayConfig { Provider = "Stripe", UseSandbox = true },
                NullLogger<StripeGateway>.Instance, _validator),
            new CashfreeGateway(new HttpClient(), new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
                NullLogger<CashfreeGateway>.Instance, _validator),
            new PayPalGateway(new HttpClient(), new GatewayConfig { Provider = "PayPal", UseSandbox = true },
                NullLogger<PayPalGateway>.Instance, _validator)
        };

        var request = new PaymentRefundRequest { GatewayPaymentId = "pay_fail_refund", Amount = 200 };

        foreach (var adapter in adapters)
        {
            var result = await adapter.RefundPaymentAsync(request);
            result.Status.Should().Be("failed", $"{adapter.ProviderName} refund should fail gracefully");
            result.GatewayPaymentId.Should().Be("pay_fail_refund");
        }
    }

    [Fact]
    public async Task PayU_Refund_AlwaysSucceeds()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentRefundRequest { GatewayPaymentId = "pay_001", Amount = 300 };

        var result = await adapter.RefundPaymentAsync(request);

        result.Status.Should().Be("processed");
        result.GatewayPaymentId.Should().Be("pay_001");
    }

    [Fact]
    public async Task AllAdapters_Cancel_ReturnsIsSuccess()
    {
        var adapters = new IPaymentGateway[]
        {
            new RazorpayGateway(new HttpClient(), new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
                NullLogger<RazorpayGateway>.Instance, _validator),
            new StripeGateway(new HttpClient(), new GatewayConfig { Provider = "Stripe", UseSandbox = true },
                NullLogger<StripeGateway>.Instance, _validator),
            new CashfreeGateway(new HttpClient(), new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
                NullLogger<CashfreeGateway>.Instance, _validator),
            new PayUGateway(new GatewayConfig { Provider = "PayU", UseSandbox = true },
                NullLogger<PayUGateway>.Instance, _validator),
            new PayPalGateway(new HttpClient(), new GatewayConfig { Provider = "PayPal", UseSandbox = true },
                NullLogger<PayPalGateway>.Instance, _validator)
        };

        var request = new PaymentCancelRequest { GatewayOrderId = "order_cancel_test", Reason = "test cancel" };

        foreach (var adapter in adapters)
        {
            var result = await adapter.CancelPaymentAsync(request);
            result.IsSuccess.Should().BeTrue($"{adapter.ProviderName} cancel should succeed");
            result.GatewayTransactionId.Should().Be("order_cancel_test");
        }
    }

    [Fact]
    public async Task AllAdapters_Cancel_WithNullReason_ReturnsIsSuccess()
    {
        var adapters = new IPaymentGateway[]
        {
            new RazorpayGateway(new HttpClient(), new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
                NullLogger<RazorpayGateway>.Instance, _validator),
            new StripeGateway(new HttpClient(), new GatewayConfig { Provider = "Stripe", UseSandbox = true },
                NullLogger<StripeGateway>.Instance, _validator),
            new CashfreeGateway(new HttpClient(), new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
                NullLogger<CashfreeGateway>.Instance, _validator),
            new PayUGateway(new GatewayConfig { Provider = "PayU", UseSandbox = true },
                NullLogger<PayUGateway>.Instance, _validator),
            new PayPalGateway(new HttpClient(), new GatewayConfig { Provider = "PayPal", UseSandbox = true },
                NullLogger<PayPalGateway>.Instance, _validator)
        };

        var request = new PaymentCancelRequest { GatewayOrderId = "" };

        foreach (var adapter in adapters)
        {
            var result = await adapter.CancelPaymentAsync(request);
            result.IsSuccess.Should().BeTrue($"{adapter.ProviderName} cancel with empty order id should succeed");
        }
    }

    [Fact]
    public async Task AllAdapters_Void_ReturnsIsSuccess()
    {
        var adapters = CreateAllAdapters();
        var request = new PaymentVoidRequest { GatewayOrderId = "order_void_test", Reason = "test void" };

        foreach (var adapter in adapters)
        {
            var result = await adapter.VoidPaymentAsync(request);
            result.IsSuccess.Should().BeTrue($"{adapter.ProviderName} void should succeed");
        }
    }

    [Fact]
    public async Task AllAdapters_GetPaymentStatus_ReturnsErrorForMissingOrder()
    {
        var adapters = CreateAllAdapters();

        foreach (var adapter in adapters)
        {
            var result = await adapter.GetPaymentStatusAsync("non_existent_order");
            if (adapter.ProviderName == "PayU")
            {
                result.Status.Should().BeOneOf("error", "unknown", $"{adapter.ProviderName} status should return error or unknown for missing order");
            }
            else
            {
                result.Status.Should().Be("error", $"{adapter.ProviderName} status should return error for missing order");
            }
        }
    }

    [Fact]
    public async Task Razorpay_CreateOrder_WithInvalidCurrency_ReturnsFailed()
    {
        var adapter = new RazorpayGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
            NullLogger<RazorpayGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest { OrderId = "order_001", Amount = 100, Currency = "" };

        var result = await adapter.CreateOrderAsync(request);

        result.Status.Should().Be("failed");
    }

    [Fact]
    public async Task PayPal_Refund_WithNonExistentPayment_ReturnsFailed()
    {
        var adapter = new PayPalGateway(
            new HttpClient(),
            new GatewayConfig { Provider = "PayPal", UseSandbox = true },
            NullLogger<PayPalGateway>.Instance,
            _validator);

        var request = new PaymentRefundRequest { GatewayPaymentId = "non_existent_paypal_order", Amount = 100 };

        var result = await adapter.RefundPaymentAsync(request);

        result.Status.Should().Be("failed");
    }

    private List<IPaymentGateway> CreateAllAdapters()
    {
        return
        [
            new RazorpayGateway(new HttpClient(), new GatewayConfig { Provider = "Razorpay", UseSandbox = true },
                NullLogger<RazorpayGateway>.Instance, _validator),
            new StripeGateway(new HttpClient(), new GatewayConfig { Provider = "Stripe", UseSandbox = true },
                NullLogger<StripeGateway>.Instance, _validator),
            new CashfreeGateway(new HttpClient(), new GatewayConfig { Provider = "Cashfree", UseSandbox = true },
                NullLogger<CashfreeGateway>.Instance, _validator),
            new PayUGateway(new GatewayConfig { Provider = "PayU", UseSandbox = true },
                NullLogger<PayUGateway>.Instance, _validator),
            new PayPalGateway(new HttpClient(), new GatewayConfig { Provider = "PayPal", UseSandbox = true },
                NullLogger<PayPalGateway>.Instance, _validator)
        ];
    }
}
