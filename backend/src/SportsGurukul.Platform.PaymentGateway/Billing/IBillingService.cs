using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Billing;

public interface IBillingService
{
    Task<InvoiceResult> GenerateInvoiceAsync(
        InvoiceGenerationRequest request,
        CancellationToken cancellationToken = default);

    Task<InvoiceResult> GenerateInvoiceWithTaxAsync(
        InvoiceGenerationRequest request,
        string? customerGstin,
        string? customerState,
        CancellationToken cancellationToken = default);

    Task<LateFeeResult> CalculateLateFeeAsync(
        decimal outstandingAmount,
        DateTime dueDate,
        DateTime asOfDate,
        decimal? lateFeeRate = null,
        decimal? penaltyRate = null,
        int? gracePeriodDays = null,
        CancellationToken cancellationToken = default);

    Task<List<InstallmentSchedule>> GenerateInstallmentPlanAsync(
        decimal totalAmount,
        int numberOfInstallments,
        DateTime startDate,
        string frequency = "monthly",
        decimal? interestRate = null,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculatePenaltyAsync(
        decimal outstandingAmount,
        int daysOverdue,
        decimal penaltyRate,
        decimal? maxPenalty = null,
        CancellationToken cancellationToken = default);

    bool IsWithinGracePeriod(DateTime dueDate, DateTime asOfDate, int gracePeriodDays);
    DateTime CalculateNextBillingDate(DateTime currentDate, string frequency, int interval = 1);
}
