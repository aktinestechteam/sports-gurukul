using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Security;

public class PaymentWebhookHandler : IPaymentWebhookHandler
{
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IPaymentSignatureValidator _signatureValidator;
    private readonly IdempotencyService _idempotencyService;
    private readonly ReplayProtectionService _replayProtection;
    private readonly ILogger<PaymentWebhookHandler> _logger;

    public event Func<WebhookPayload, Task>? OnPaymentSuccess;
    public event Func<WebhookPayload, Task>? OnPaymentFailed;
    public event Func<WebhookPayload, Task>? OnPaymentCaptured;
    public event Func<WebhookPayload, Task>? OnRefundCompleted;
    public event Func<WebhookPayload, Task>? OnDisputeCreated;
    public event Func<WebhookPayload, Task>? OnChargeback;

    public PaymentWebhookHandler(
        IPaymentGatewayFactory gatewayFactory,
        IPaymentSignatureValidator signatureValidator,
        IdempotencyService idempotencyService,
        ReplayProtectionService replayProtection,
        ILogger<PaymentWebhookHandler> logger)
    {
        _gatewayFactory = gatewayFactory;
        _signatureValidator = signatureValidator;
        _idempotencyService = idempotencyService;
        _replayProtection = replayProtection;
        _logger = logger;
    }

    public async Task<WebhookResult> HandleWebhookAsync(
        WebhookPayload payload,
        string providerName,
        CancellationToken cancellationToken = default)
    {
        var webhookId = payload.WebhookId ?? payload.IdempotencyKey ?? $"{providerName}_{payload.GatewayPaymentId}";

        if (_replayProtection.IsReplayAttack(webhookId, webhookId, payload.WebhookTimestamp ?? DateTime.UtcNow))
        {
            _logger.LogWarning("Replay attack detected for webhook {WebhookId}", webhookId);
            return new WebhookResult
            {
                IsProcessed = false,
                EventType = WebhookEventType.Unknown,
                ErrorMessage = "Replay attack detected"
            };
        }

        if (!string.IsNullOrEmpty(payload.IdempotencyKey) && _idempotencyService.TryGetResult(payload.IdempotencyKey, out var cached))
        {
            _logger.LogInformation("Duplicate webhook received (idempotent): {IdempotencyKey}", payload.IdempotencyKey);
            var cachedResult = cached as WebhookResult;
            if (cachedResult is not null)
            {
                cachedResult.IsIdempotent = true;
                return cachedResult;
            }
            return new WebhookResult
            {
                IsProcessed = true,
                IsIdempotent = true,
                EventType = DetermineEventType(payload.EventType)
            };
        }

        var eventType = DetermineEventType(payload.EventType);

        var result = eventType switch
        {
            WebhookEventType.PaymentSuccess => await HandlePaymentSuccessAsync(payload, cancellationToken),
            WebhookEventType.PaymentFailed => await HandlePaymentFailedAsync(payload, cancellationToken),
            WebhookEventType.PaymentCaptured => await HandlePaymentCapturedAsync(payload, cancellationToken),
            WebhookEventType.RefundCompleted => await HandleRefundCompletedAsync(payload, cancellationToken),
            WebhookEventType.DisputeCreated => await HandleDisputeCreatedAsync(payload, cancellationToken),
            WebhookEventType.Chargeback => await HandleChargebackAsync(payload, cancellationToken),
            _ => new WebhookResult
            {
                IsProcessed = true,
                EventType = eventType,
                ErrorMessage = $"Unhandled event type: {payload.EventType}"
            }
        };

        if (!string.IsNullOrEmpty(payload.IdempotencyKey))
            _idempotencyService.TrySetResult(payload.IdempotencyKey, result);

        return result;
    }

    public Task<WebhookResult> HandlePaymentSuccessAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Payment success webhook: {GatewayPaymentId}", payload.GatewayPaymentId);
        OnPaymentSuccess?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.PaymentSuccess,
            GatewayTransactionId = payload.GatewayPaymentId
        });
    }

    public Task<WebhookResult> HandlePaymentFailedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Payment failed webhook: {GatewayPaymentId}, Reason: {FailureReason}",
            payload.GatewayPaymentId, payload.FailureReason);
        OnPaymentFailed?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.PaymentFailed,
            GatewayTransactionId = payload.GatewayPaymentId
        });
    }

    public Task<WebhookResult> HandlePaymentCapturedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Payment captured webhook: {GatewayPaymentId}", payload.GatewayPaymentId);
        OnPaymentCaptured?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.PaymentCaptured,
            GatewayTransactionId = payload.GatewayPaymentId
        });
    }

    public Task<WebhookResult> HandleRefundCompletedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refund completed webhook: {GatewayRefundId}", payload.GatewayRefundId);
        OnRefundCompleted?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.RefundCompleted,
            GatewayTransactionId = payload.GatewayRefundId
        });
    }

    public Task<WebhookResult> HandleDisputeCreatedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Dispute created webhook: {DisputeId}, Reason: {DisputeReason}",
            payload.DisputeId, payload.DisputeReason);
        OnDisputeCreated?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.DisputeCreated,
            GatewayTransactionId = payload.DisputeId
        });
    }

    public Task<WebhookResult> HandleChargebackAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Chargeback webhook: {DisputeId}", payload.DisputeId);
        OnChargeback?.Invoke(payload);
        return Task.FromResult(new WebhookResult
        {
            IsProcessed = true,
            EventType = WebhookEventType.Chargeback,
            GatewayTransactionId = payload.DisputeId
        });
    }

    private static WebhookEventType DetermineEventType(string eventType)
    {
        return eventType?.ToLowerInvariant() switch
        {
            "payment.success" or "payment_intent.succeeded" or "paid" or "payment_captured" => WebhookEventType.PaymentSuccess,
            "payment.failed" or "payment_intent.payment_failed" or "failed" => WebhookEventType.PaymentFailed,
            "payment.captured" or "capture.completed" => WebhookEventType.PaymentCaptured,
            "payment.authorized" or "authorized" => WebhookEventType.PaymentAuthorized,
            "refund.completed" or "refund.created" or "refunded" => WebhookEventType.RefundCompleted,
            "refund.failed" => WebhookEventType.RefundFailed,
            "dispute.created" => WebhookEventType.DisputeCreated,
            "dispute.resolved" => WebhookEventType.DisputeResolved,
            "chargeback" => WebhookEventType.Chargeback,
            "order.created" => WebhookEventType.OrderCreated,
            "order.expired" => WebhookEventType.OrderExpired,
            _ => WebhookEventType.Unknown
        };
    }
}
