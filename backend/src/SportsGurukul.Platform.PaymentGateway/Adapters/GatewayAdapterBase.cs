using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Adapters;

public abstract class GatewayAdapterBase : IPaymentGateway
{
    protected readonly ILogger Logger;
    protected readonly IPaymentSignatureValidator SignatureValidator;

    public abstract string ProviderName { get; }

    protected GatewayAdapterBase(
        ILogger logger,
        IPaymentSignatureValidator signatureValidator)
    {
        Logger = logger;
        SignatureValidator = signatureValidator;
    }

    public abstract Task<PaymentOrderResponse> CreateOrderAsync(
        PaymentOrderRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentOrderResponse> AuthorizePaymentAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentOrderResponse> CapturePaymentAsync(
        PaymentCaptureRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentStatusResponse> GetPaymentStatusAsync(
        string gatewayOrderId,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<GatewayOperationResult> CancelPaymentAsync(
        PaymentCancelRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<GatewayOperationResult> VoidPaymentAsync(
        PaymentVoidRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentOrderResponse> RetryPaymentAsync(
        PaymentRetryRequest request,
        CancellationToken cancellationToken = default);

    public abstract Task<GatewayOperationResult> VerifyWebhookSignatureAsync(
        WebhookPayload payload,
        CancellationToken cancellationToken = default);

    public abstract Task<PaymentMethodToken?> SavePaymentMethodAsync(
        string customerId,
        string gatewayPaymentMethodId,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> DeletePaymentMethodAsync(
        string tokenId,
        CancellationToken cancellationToken = default);

    protected static PaymentOrderResponse CreateErrorResponse(string errorMessage, string providerOrderId = "")
    {
        return new PaymentOrderResponse
        {
            ProviderOrderId = providerOrderId,
            Status = "failed"
        };
    }

    protected static PaymentStatusResponse CreateErrorStatusResponse(string errorMessage)
    {
        return new PaymentStatusResponse
        {
            Status = "error"
        };
    }
}
