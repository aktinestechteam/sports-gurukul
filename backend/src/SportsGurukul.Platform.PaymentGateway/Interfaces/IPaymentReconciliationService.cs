using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Interfaces;

public interface IPaymentReconciliationService
{
    Task<bool> ReconcilePaymentAsync(
        string gatewayOrderId,
        string provider,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> FindUnreconciledPaymentsAsync(
        string provider,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    Task<bool> ReconcileDiscrepancyAsync(
        string gatewayOrderId,
        decimal expectedAmount,
        decimal actualAmount,
        string provider,
        CancellationToken cancellationToken = default);

    Task<GatewayOperationResult> SubmitForSettlementAsync(
        string gatewayOrderId,
        string provider,
        CancellationToken cancellationToken = default);
}
