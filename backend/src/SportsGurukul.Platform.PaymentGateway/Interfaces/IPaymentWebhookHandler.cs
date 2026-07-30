using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentWebhookHandler
{
    Task<WebhookResult> HandleWebhookAsync(
        WebhookPayload payload,
        string providerName,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandlePaymentSuccessAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandlePaymentFailedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandlePaymentCapturedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandleRefundCompletedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandleDisputeCreatedAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> HandleChargebackAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    event Func<WebhookPayload, Task>? OnPaymentSuccess;
    event Func<WebhookPayload, Task>? OnPaymentFailed;
    event Func<WebhookPayload, Task>? OnPaymentCaptured;
    event Func<WebhookPayload, Task>? OnRefundCompleted;
    event Func<WebhookPayload, Task>? OnDisputeCreated;
    event Func<WebhookPayload, Task>? OnChargeback;
}
