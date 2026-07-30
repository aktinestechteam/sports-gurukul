using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Adapters;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class GatewayAdapterTests
{
    private readonly IPaymentSignatureValidator _validator;

    public GatewayAdapterTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
    }

    [Fact]
    public async Task CreateOrder_WithStub_ReturnsOrderResponse()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentOrderRequest
        {
            OrderId = "order_001",
            Amount = 1000,
            Currency = "INR",
            Description = "Test order"
        };

        var result = await adapter.CreateOrderAsync(request);

        Assert.NotNull(result);
        Assert.Equal("order_001", result.ProviderOrderId);
        Assert.Equal("created", result.Status);
        Assert.Equal(1000, result.Amount);
    }

    [Fact]
    public async Task PayU_GetPaymentStatus_ReturnsStatusResponse()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var result = await adapter.GetPaymentStatusAsync("order_001");

        Assert.NotNull(result);
        Assert.Equal("order_001", result.GatewayOrderId);
    }

    [Fact]
    public async Task PayU_Refund_ReturnsRefundResponse()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var request = new PaymentRefundRequest
        {
            GatewayPaymentId = "pay_001",
            Amount = 500,
            Reason = "test refund"
        };

        var result = await adapter.RefundPaymentAsync(request);

        Assert.NotNull(result);
        Assert.Equal("pay_001", result.GatewayPaymentId);
        Assert.Equal("processed", result.Status);
    }

    [Fact]
    public async Task AllAdapters_Cancel_ReturnsSuccess()
    {
        var adapters = CreateAllAdapters();

        foreach (var adapter in adapters)
        {
            var result = await adapter.CancelPaymentAsync(new PaymentCancelRequest
            {
                GatewayOrderId = "order_001",
                Reason = "test cancel"
            });

            Assert.True(result.IsSuccess, $"{adapter.ProviderName} cancel failed");
        }
    }

    [Fact]
    public async Task AllAdapters_Void_ReturnsSuccess()
    {
        var adapters = CreateAllAdapters();

        foreach (var adapter in adapters)
        {
            var result = await adapter.VoidPaymentAsync(new PaymentVoidRequest
            {
                GatewayOrderId = "order_001",
                Reason = "test void"
            });

            Assert.True(result.IsSuccess, $"{adapter.ProviderName} void failed");
        }
    }

    [Fact]
    public async Task AllAdapters_SavePaymentMethod_ReturnsToken()
    {
        var adapters = CreateAllAdapters();

        foreach (var adapter in adapters)
        {
            var token = await adapter.SavePaymentMethodAsync("cust_001", "pm_001");
            Assert.NotNull(token);
            Assert.Equal(adapter.ProviderName, token.Provider);
        }
    }

    [Fact]
    public async Task AllAdapters_DeletePaymentMethod_ReturnsTrue()
    {
        var adapters = CreateAllAdapters();

        foreach (var adapter in adapters)
        {
            var result = await adapter.DeletePaymentMethodAsync("tok_001");
            Assert.True(result, $"{adapter.ProviderName} delete failed");
        }
    }

    [Fact]
    public async Task RetryPayment_CreatesNewOrder()
    {
        var adapter = new PayUGateway(
            new GatewayConfig { Provider = "PayU", UseSandbox = true },
            NullLogger<PayUGateway>.Instance,
            _validator);

        var result = await adapter.RetryPaymentAsync(new PaymentRetryRequest
        {
            GatewayOrderId = "order_001",
            NewIdempotencyKey = "idem_002"
        });

        Assert.NotNull(result);
        Assert.Equal("created", result.Status);
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
