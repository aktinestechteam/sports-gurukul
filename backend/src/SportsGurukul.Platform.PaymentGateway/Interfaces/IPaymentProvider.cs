using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentProvider
{
    string ProviderCode { get; }
    string DisplayName { get; }
    bool IsActive { get; }

    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> RetryPaymentAsync(
        PaymentRetryRequest request,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<WebhookResult> ProcessWebhookAsync(
        WebhookPayload payload,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    string GenerateSignature(string payload, string secret);
    bool ValidateSignature(string payload, string signature, string secret);

    Task<PaymentMethodToken?> TokenizePaymentMethodAsync(
        string customerId,
        string gatewayPaymentMethodId,
        GatewayConfig config,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveTokenizedMethodAsync(
        string gatewayTokenId,
        GatewayConfig config,
        CancellationToken cancellationToken = default);
}
