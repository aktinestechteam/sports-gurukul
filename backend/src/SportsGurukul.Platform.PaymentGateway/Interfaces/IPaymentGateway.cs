using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentGateway
{
    string ProviderName { get; }

    Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default);

    Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentOrderResponse> RetryPaymentAsync(
        PaymentRetryRequest request,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> VerifyWebhookSignatureAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    Task<PaymentMethodToken?> SavePaymentMethodAsync(
        string customerId,
        string gatewayPaymentMethodId,
        CancellationToken cancellationToken = default);

    Task<bool> DeletePaymentMethodAsync(
        string tokenId,
        CancellationToken cancellationToken = default);
}
