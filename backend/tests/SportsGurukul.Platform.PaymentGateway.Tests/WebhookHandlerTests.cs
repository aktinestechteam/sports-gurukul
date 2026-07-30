using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Factory;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class WebhookHandlerTests
{
    private readonly IPaymentWebhookHandler _handler;
    private readonly IPaymentSignatureValidator _validator;
    private readonly IdempotencyService _idempotency;
    private readonly ReplayProtectionService _replayProtection;
    private readonly PaymentGatewayFactory _factory;

    public WebhookHandlerTests()
    {
        _validator = new WebhookSignatureValidator(NullLogger<WebhookSignatureValidator>.Instance);
        _idempotency = new IdempotencyService(NullLogger<IdempotencyService>.Instance);
        _replayProtection = new ReplayProtectionService(NullLogger<ReplayProtectionService>.Instance);
        _factory = new PaymentGatewayFactory(NullLogger<PaymentGatewayFactory>.Instance);

        _handler = new PaymentWebhookHandler(
            _factory,
            _validator,
            _idempotency,
            _replayProtection,
            NullLogger<PaymentWebhookHandler>.Instance);
    }

    [Fact]
    public async Task HandlePaymentSuccessWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_001",
            Amount = 1000,
            Currency = "INR",
            WebhookId = "wh_001",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.PaymentSuccess, result.EventType);
        Assert.Equal("pay_001", result.GatewayTransactionId);
    }

    [Fact]
    public async Task HandlePaymentFailedWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.failed",
            GatewayPaymentId = "pay_002",
            FailureReason = "insufficient_funds",
            WebhookId = "wh_002",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.PaymentFailed, result.EventType);
    }

    [Fact]
    public async Task HandlePaymentCapturedWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.captured",
            GatewayPaymentId = "pay_003",
            Amount = 500,
            WebhookId = "wh_003",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.PaymentCaptured, result.EventType);
    }

    [Fact]
    public async Task HandleRefundCompletedWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "refund.completed",
            GatewayRefundId = "rf_001",
            GatewayPaymentId = "pay_001",
            Amount = 200,
            WebhookId = "wh_004",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.RefundCompleted, result.EventType);
    }

    [Fact]
    public async Task HandleDisputeCreatedWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "dispute.created",
            DisputeId = "disp_001",
            DisputeReason = "fraudulent",
            WebhookId = "wh_005",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.DisputeCreated, result.EventType);
    }

    [Fact]
    public async Task HandleChargebackWebhook_ShouldReturnProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "chargeback",
            DisputeId = "cb_001",
            GatewayPaymentId = "pay_001",
            WebhookId = "wh_006",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.Chargeback, result.EventType);
    }

    [Fact]
    public async Task HandleUnknownEventType_ShouldReturnProcessedWithWarning()
    {
        var payload = new WebhookPayload
        {
            EventType = "order.created",
            GatewayPaymentId = "pay_004",
            WebhookId = "wh_007",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        Assert.True(result.IsProcessed);
        Assert.Equal(WebhookEventType.OrderCreated, result.EventType);
    }

    [Fact]
    public async Task DuplicateWebhook_WithIdempotencyKey_ShouldReturnCachedResult()
    {
        var payload1 = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_005",
            IdempotencyKey = "idem_001",
            WebhookId = "wh_008",
            WebhookTimestamp = DateTime.UtcNow
        };

        var payload2 = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_005",
            IdempotencyKey = "idem_001",
            WebhookId = "wh_009",
            WebhookTimestamp = DateTime.UtcNow
        };

        var first = await _handler.HandleWebhookAsync(payload1, "razorpay");
        var second = await _handler.HandleWebhookAsync(payload2, "razorpay");

        Assert.True(first.IsProcessed);
        Assert.True(second.IsIdempotent);
    }

    [Fact]
    public async Task ReplayAttack_ShouldBeDetected()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_006",
            WebhookId = "wh_replay",
            WebhookTimestamp = DateTime.UtcNow
        };

        await _handler.HandleWebhookAsync(payload, "razorpay");

        var replayPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_006",
            WebhookId = "wh_replay",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(replayPayload, "razorpay");

        Assert.False(result.IsProcessed);
        Assert.Equal("Replay attack detected", result.ErrorMessage);
    }

    [Fact]
    public async Task StripeEventType_ShouldMapCorrectly()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment_intent.succeeded",
            GatewayPaymentId = "pi_001",
            WebhookId = "wh_stripe",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "stripe");
        Assert.Equal(WebhookEventType.PaymentSuccess, result.EventType);
    }

    [Fact]
    public async Task Events_ShouldBeInvoked()
    {
        var invoked = false;
        _handler.OnPaymentSuccess += payload =>
        {
            invoked = true;
            return Task.CompletedTask;
        };

        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_007",
            WebhookId = "wh_event",
            WebhookTimestamp = DateTime.UtcNow
        };

        await _handler.HandleWebhookAsync(payload, "razorpay");
        Assert.True(invoked);
    }
}
