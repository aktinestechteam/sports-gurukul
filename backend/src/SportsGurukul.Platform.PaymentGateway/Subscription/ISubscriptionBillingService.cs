using SportsGurukul.Platform.PaymentGateway.Models;

namespace SportsGurukul.Platform.PaymentGateway.Subscription;

public interface ISubscriptionBillingService
{
    Task<RecurringBillingProfile> CreateProfileAsync(
        RecurringBillingProfile profile,
        CancellationToken cancellationToken = default);

    Task<RecurringBillingProfile?> GetProfileAsync(
        string profileId,
        CancellationToken cancellationToken = default);

    Task<RecurringBillingProfile> UpdateProfileAsync(
        RecurringBillingProfile profile,
        CancellationToken cancellationToken = default);

    Task<bool> CancelProfileAsync(
        string profileId,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<bool> PauseProfileAsync(
        string profileId,
        CancellationToken cancellationToken = default);

    Task<bool> ResumeProfileAsync(
        string profileId,
        CancellationToken cancellationToken = default);

    Task<InvoiceResult> GenerateSubscriptionInvoiceAsync(
        string profileId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecurringBillingProfile>> GetDueProfilesAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default);
}
