using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Subscription;

public interface IRecurringInvoiceService
{
    Task<InvoiceResult> GenerateRecurringInvoiceAsync(
        string profileId,
        int cycleNumber,
        CancellationToken cancellationToken = default);

    Task<InvoiceResult> GenerateCatchUpInvoiceAsync(
        string profileId,
        int missedCycles,
        CancellationToken cancellationToken = default);

    Task<InvoiceResult> GenerateProratedInvoiceAsync(
        string profileId,
        DateTime fromDate,
        DateTime toDate,
        decimal proratedAmount,
        CancellationToken cancellationToken = default);

    Task<bool> SkipBillingCycleAsync(
        string profileId,
        int cycleNumber,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculateProratedAmountAsync(
        decimal fullAmount,
        DateTime cycleStart,
        DateTime cycleEnd,
        DateTime effectiveDate,
        CancellationToken cancellationToken = default);
}
