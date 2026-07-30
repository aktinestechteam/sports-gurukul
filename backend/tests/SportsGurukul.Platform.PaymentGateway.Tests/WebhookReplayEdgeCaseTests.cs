using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Platform.PaymentGateway.Factory;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;

namespace SportsGurukul.Platform.PaymentGateway.Tests;

public class WebhookReplayEdgeCaseTests
{
    private readonly IPaymentWebhookHandler _handler;
    private readonly IPaymentSignatureValidator _validator;
    private readonly IdempotencyService _idempotency;
    private readonly ReplayProtectionService _replayProtection;
    private readonly PaymentGatewayFactory _factory;

    public WebhookReplayEdgeCaseTests()
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
    public async Task SameWebhookId_ReplayedAfterLongDelay_ShouldBeDetected()
    {
        var firstPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_replay_delay",
            WebhookId = "wh_replay_delay",
            WebhookTimestamp = DateTime.UtcNow
        };

        var firstResult = await _handler.HandleWebhookAsync(firstPayload, "razorpay");
        firstResult.IsProcessed.Should().BeTrue();

        var secondPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_replay_delay",
            WebhookId = "wh_replay_delay",
            WebhookTimestamp = DateTime.UtcNow
        };

        var secondResult = await _handler.HandleWebhookAsync(secondPayload, "razorpay");

        secondResult.IsProcessed.Should().BeFalse();
        secondResult.ErrorMessage.Should().Be("Replay attack detected");
    }

    [Fact]
    public async Task Replay_WithDifferentPayloadButSameWebhookId_ShouldBeDetected()
    {
        var firstPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_diff_payload",
            Amount = 1000,
            WebhookId = "wh_diff_payload",
            WebhookTimestamp = DateTime.UtcNow
        };

        await _handler.HandleWebhookAsync(firstPayload, "razorpay");

        var secondPayload = new WebhookPayload
        {
            EventType = "payment.captured",
            GatewayPaymentId = "pay_diff_payload",
            Amount = 500,
            WebhookId = "wh_diff_payload",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(secondPayload, "razorpay");

        result.IsProcessed.Should().BeFalse();
        result.ErrorMessage.Should().Be("Replay attack detected");
    }

    [Fact]
    public async Task ReplayDetection_AcrossDifferentGatewayProviders_ShouldBeDetected()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_cross_provider",
            WebhookId = "wh_cross_provider",
            WebhookTimestamp = DateTime.UtcNow
        };

        await _handler.HandleWebhookAsync(payload, "razorpay");

        var replayPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_cross_provider",
            WebhookId = "wh_cross_provider",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(replayPayload, "stripe");

        result.IsProcessed.Should().BeFalse();
        result.ErrorMessage.Should().Be("Replay attack detected");
    }

    [Fact]
    public async Task Webhook_WithMissingWebhookId_ShouldBeProcessed()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_no_webhook_id",
            WebhookTimestamp = DateTime.UtcNow
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        result.IsProcessed.Should().BeTrue();
        result.EventType.Should().Be(WebhookEventType.PaymentSuccess);
    }

    [Fact]
    public async Task Webhook_WithExtremeFutureTimestamp_ShouldBeDetectedAsReplay()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_future_ts",
            WebhookId = "wh_future_ts",
            WebhookTimestamp = DateTime.UtcNow.AddYears(1)
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        result.IsProcessed.Should().BeFalse();
        result.ErrorMessage.Should().Be("Replay attack detected");
    }

    [Fact]
    public async Task Webhook_WithExtremePastTimestamp_ShouldBeDetectedAsReplay()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_past_ts",
            WebhookId = "wh_past_ts",
            WebhookTimestamp = DateTime.UtcNow.AddDays(-30)
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        result.IsProcessed.Should().BeFalse();
        result.ErrorMessage.Should().Be("Replay attack detected");
    }

    [Fact]
    public async Task Webhook_WithNullTimestamp_ShouldNotTriggerReplayOnFirstCall()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_null_ts",
            WebhookId = "wh_null_ts",
            WebhookTimestamp = null
        };

        var result = await _handler.HandleWebhookAsync(payload, "razorpay");

        result.IsProcessed.Should().BeTrue();
    }

    [Fact]
    public async Task ConcurrentWebhooks_SameId_OnlyFirstShouldProcess()
    {
        var payload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_concurrent",
            WebhookId = "wh_concurrent",
            WebhookTimestamp = DateTime.UtcNow
        };

        var tasks = new[]
        {
            _handler.HandleWebhookAsync(payload, "razorpay"),
            _handler.HandleWebhookAsync(payload, "razorpay"),
            _handler.HandleWebhookAsync(payload, "razorpay")
        };

        var results = await Task.WhenAll(tasks);

        var processedCount = results.Count(r => r.IsProcessed);
        var replayCount = results.Count(r => !r.IsProcessed);

        processedCount.Should().Be(1);
        replayCount.Should().Be(2);
    }

    [Fact]
    public async Task DistinctWebhookIds_ShouldAllBeProcessed()
    {
        var payloads = new[]
        {
            new WebhookPayload { EventType = "payment.success", GatewayPaymentId = "pay_distinct_1", WebhookId = "wh_distinct_1", WebhookTimestamp = DateTime.UtcNow },
            new WebhookPayload { EventType = "payment.success", GatewayPaymentId = "pay_distinct_2", WebhookId = "wh_distinct_2", WebhookTimestamp = DateTime.UtcNow },
            new WebhookPayload { EventType = "payment.success", GatewayPaymentId = "pay_distinct_3", WebhookId = "wh_distinct_3", WebhookTimestamp = DateTime.UtcNow }
        };

        foreach (var payload in payloads)
        {
            var result = await _handler.HandleWebhookAsync(payload, "razorpay");
            result.IsProcessed.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Replay_WithSameIdempotencyKey_ReturnsCachedNotReplay()
    {
        var firstPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_idem_replay",
            IdempotencyKey = "idem_replay_001",
            WebhookId = "wh_idem_replay_1",
            WebhookTimestamp = DateTime.UtcNow
        };

        var secondPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_idem_replay",
            IdempotencyKey = "idem_replay_001",
            WebhookId = "wh_idem_replay_2",
            WebhookTimestamp = DateTime.UtcNow
        };

        var first = await _handler.HandleWebhookAsync(firstPayload, "razorpay");
        var second = await _handler.HandleWebhookAsync(secondPayload, "razorpay");

        first.IsProcessed.Should().BeTrue();
        second.IsIdempotent.Should().BeTrue();
    }

    [Fact]
    public async Task SameWebhookId_DifferentProviders_ShouldNotInterfere()
    {
        var razorpayPayload = new WebhookPayload
        {
            EventType = "payment.success",
            GatewayPaymentId = "pay_provider_specific",
            WebhookId = "wh_provider_shared",
            WebhookTimestamp = DateTime.UtcNow
        };

        var stripePayload = new WebhookPayload
        {
            EventType = "payment_intent.succeeded",
            GatewayPaymentId = "pay_provider_specific",
            WebhookId = "wh_provider_shared",
            WebhookTimestamp = DateTime.UtcNow
        };

        var first = await _handler.HandleWebhookAsync(razorpayPayload, "razorpay");
        first.IsProcessed.Should().BeTrue();

        var second = await _handler.HandleWebhookAsync(stripePayload, "stripe");
        second.IsProcessed.Should().BeFalse();
        second.ErrorMessage.Should().Be("Replay attack detected");
    }
}
