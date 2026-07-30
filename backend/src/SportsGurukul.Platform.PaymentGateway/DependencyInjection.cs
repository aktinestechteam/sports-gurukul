using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.PaymentGateway.Accounting;
using SportsGurukul.Platform.PaymentGateway.Adapters;
using SportsGurukul.Platform.PaymentGateway.Billing;
using SportsGurukul.Platform.PaymentGateway.Discount;
using SportsGurukul.Platform.PaymentGateway.Factory;
using SportsGurukul.Platform.PaymentGateway.Fraud;
using SportsGurukul.Platform.PaymentGateway.Interfaces;
using SportsGurukul.Platform.PaymentGateway.Models;
using SportsGurukul.Platform.PaymentGateway.Security;
using SportsGurukul.Platform.PaymentGateway.Subscription;
using SportsGurukul.Platform.PaymentGateway.Tax;

namespace SportsGurukul.Platform.PaymentGateway;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentGatewayPlatform(
        this IServiceCollection services,
        Action<PaymentGatewayOptions>? configureOptions = null)
    {
        var options = new PaymentGatewayOptions();
        configureOptions?.Invoke(options);

        // Core services
        services.AddSingleton<IPaymentSignatureValidator, WebhookSignatureValidator>();
        services.AddSingleton<IdempotencyService>();
        services.AddSingleton<ReplayProtectionService>();
        services.AddSingleton<IPaymentTokenService, PaymentTokenService>();
        services.AddSingleton<IPaymentWebhookHandler, PaymentWebhookHandler>();
        services.AddSingleton<IPaymentGatewayFactory, PaymentGatewayFactory>();

        // Billing Engine
        services.AddSingleton<ITaxEngine, TaxEngine>();
        services.AddSingleton<IBillingService, BillingService>();
        services.AddSingleton<IDiscountEngine, DiscountEngine>();

        // Discount handlers
        services.AddSingleton<IDiscountHandler, CouponDiscountHandler>();
        services.AddSingleton<IDiscountHandler, ScholarshipDiscountHandler>();
        services.AddSingleton<IDiscountHandler, PromotionDiscountHandler>();

        // Accounting
        services.AddSingleton<IAccountingService, AccountingService>();

        // Subscription (interfaces only)
        services.AddSingleton<ISubscriptionBillingService, StubSubscriptionBillingService>();
        services.AddSingleton<IRecurringInvoiceService, StubRecurringInvoiceService>();

        // Payment Reconciliation
        services.AddSingleton<IPaymentReconciliationService, StubPaymentReconciliationService>();

        // Fraud detection (stub implementations)
        services.AddSingleton<IFraudDetectionService, StubFraudDetectionService>();
        services.AddSingleton<IRiskAssessmentService, StubRiskAssessmentService>();

        // Register post-configuration to wire gateways into factory
        services.AddSingleton<PaymentGatewayFactory>(sp =>
        {
            var factory = (PaymentGatewayFactory)sp.GetRequiredService<IPaymentGatewayFactory>();
            if (factory is null) throw new InvalidOperationException("PaymentGatewayFactory not registered");

            var logger = sp.GetRequiredService<ILogger<PaymentGatewayFactory>>();
            var validator = sp.GetRequiredService<IPaymentSignatureValidator>();

            RegisterGateway(factory, sp, logger, validator, "Razorpay", options,
                (c, v) => new RazorpayGateway(CreateHttpClient(c, "Razorpay"), c,
                    sp.GetRequiredService<ILogger<RazorpayGateway>>(), v));

            RegisterGateway(factory, sp, logger, validator, "Stripe", options,
                (c, v) => new StripeGateway(CreateHttpClient(c, "Stripe"), c,
                    sp.GetRequiredService<ILogger<StripeGateway>>(), v));

            RegisterGateway(factory, sp, logger, validator, "Cashfree", options,
                (c, v) => new CashfreeGateway(CreateHttpClient(c, "Cashfree"), c,
                    sp.GetRequiredService<ILogger<CashfreeGateway>>(), v));

            RegisterGateway(factory, sp, logger, validator, "PayU", options,
                (c, v) => new PayUGateway(c,
                    sp.GetRequiredService<ILogger<PayUGateway>>(), v));

            RegisterGateway(factory, sp, logger, validator, "PayPal", options,
                (c, v) => new PayPalGateway(CreateHttpClient(c, "PayPal"), c,
                    sp.GetRequiredService<ILogger<PayPalGateway>>(), v));

            if (!string.IsNullOrEmpty(options.DefaultProvider))
                factory.SetDefaultProvider(options.DefaultProvider);

            return factory;
        });

        return services;
    }

    private static void RegisterGateway(
        PaymentGatewayFactory factory,
        IServiceProvider sp,
        ILogger<PaymentGatewayFactory> logger,
        IPaymentSignatureValidator validator,
        string providerName,
        PaymentGatewayOptions options,
        Func<GatewayConfig, IPaymentSignatureValidator, IPaymentGateway> createGateway)
    {
        var config = options.GetProviderConfig(providerName) ?? new GatewayConfig
        {
            Provider = providerName,
            UseSandbox = true
        };

        var gateway = createGateway(config, validator);

        factory.RegisterProvider(providerName, gateway);
        logger.LogInformation("Registered {Provider} payment gateway", providerName);
    }

    private static HttpClient CreateHttpClient(GatewayConfig config, string providerName)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds > 0 ? config.TimeoutSeconds : 30)
        };
        return client;
    }
}

public class PaymentGatewayOptions
{
    private readonly Dictionary<string, GatewayConfig> _providerConfigs = new(StringComparer.OrdinalIgnoreCase);

    public string? DefaultProvider { get; set; }

    public PaymentGatewayOptions ConfigureProvider(string name, GatewayConfig config)
    {
        _providerConfigs[name] = config;
        return this;
    }

    public PaymentGatewayOptions ConfigureProvider(string name, Action<GatewayConfig> configure)
    {
        var config = new GatewayConfig { Provider = name };
        configure(config);
        _providerConfigs[name] = config;
        return this;
    }

    public GatewayConfig? GetProviderConfig(string name)
    {
        return _providerConfigs.TryGetValue(name, out var config) ? config : null;
    }

    public Dictionary<string, GatewayConfig> GetAllConfigs()
    {
        return new Dictionary<string, GatewayConfig>(_providerConfigs, StringComparer.OrdinalIgnoreCase);
    }
}

internal class StubSubscriptionBillingService : ISubscriptionBillingService
{
    public Task<RecurringBillingProfile> CreateProfileAsync(RecurringBillingProfile profile, CancellationToken cancellationToken = default)
    {
        profile.ProfileId = $"sub_{Guid.NewGuid():N}";
        return Task.FromResult(profile);
    }

    public Task<RecurringBillingProfile?> GetProfileAsync(string profileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<RecurringBillingProfile?>(null);
    }

    public Task<RecurringBillingProfile> UpdateProfileAsync(RecurringBillingProfile profile, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(profile);
    }

    public Task<bool> CancelProfileAsync(string profileId, string? reason = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> PauseProfileAsync(string profileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> ResumeProfileAsync(string profileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<InvoiceResult> GenerateSubscriptionInvoiceAsync(string profileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new InvoiceResult { InvoiceNumber = $"SUB-{profileId}" });
    }

    public Task<IReadOnlyList<RecurringBillingProfile>> GetDueProfilesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<RecurringBillingProfile>>([]);
    }
}

internal class StubRecurringInvoiceService : IRecurringInvoiceService
{
    public Task<InvoiceResult> GenerateRecurringInvoiceAsync(string profileId, int cycleNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new InvoiceResult { InvoiceNumber = $"REC-{profileId}-{cycleNumber}" });
    }

    public Task<InvoiceResult> GenerateCatchUpInvoiceAsync(string profileId, int missedCycles, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new InvoiceResult { InvoiceNumber = $"CATCH-{profileId}" });
    }

    public Task<InvoiceResult> GenerateProratedInvoiceAsync(string profileId, DateTime fromDate, DateTime toDate, decimal proratedAmount, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new InvoiceResult { InvoiceNumber = $"PRO-{profileId}" });
    }

    public Task<bool> SkipBillingCycleAsync(string profileId, int cycleNumber, string? reason = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<decimal> CalculateProratedAmountAsync(decimal fullAmount, DateTime cycleStart, DateTime cycleEnd, DateTime effectiveDate, CancellationToken cancellationToken = default)
    {
        var totalDays = (cycleEnd - cycleStart).TotalDays;
        var remainingDays = (cycleEnd - effectiveDate).TotalDays;
        if (totalDays <= 0) return Task.FromResult(fullAmount);
        return Task.FromResult(Math.Round(fullAmount * (decimal)(remainingDays / totalDays), 2));
    }
}

internal class StubPaymentReconciliationService : IPaymentReconciliationService
{
    public Task<bool> ReconcilePaymentAsync(string gatewayOrderId, string provider, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IReadOnlyList<string>> FindUnreconciledPaymentsAsync(string provider, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([]);
    }

    public Task<bool> ReconcileDiscrepancyAsync(string gatewayOrderId, decimal expectedAmount, decimal actualAmount, string provider, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<GatewayOperationResult> SubmitForSettlementAsync(string gatewayOrderId, string provider, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GatewayOperationResult { IsSuccess = true });
    }
}

internal class StubFraudDetectionService : IFraudDetectionService
{
    public Task<FraudAssessmentResult> AssessAsync(FraudAssessmentRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new FraudAssessmentResult
        {
            IsFraudulent = false,
            RiskScore = 0,
            RiskLevel = "low",
            AssessmentId = $"fraud_{Guid.NewGuid():N}"
        });
    }

    public Task<bool> IsSuspiciousAsync(string customerId, decimal amount, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> IsBlacklistedAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> IsHighRiskTransactionAsync(decimal amount, string paymentMethod, string? customerId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}

internal class StubRiskAssessmentService : IRiskAssessmentService
{
    public Task<RiskScore> CalculateRiskScoreAsync(RiskAssessmentRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new RiskScore { Score = 0, Level = RiskLevel.Low });
    }

    public Task<bool> RequiresAdditionalVerificationAsync(string customerId, decimal amount, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<RiskLevel> DetermineRiskLevelAsync(decimal riskScore, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(RiskLevel.Low);
    }

    public Task<IReadOnlyList<string>> GetRiskFlagsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<string>>([]);
    }
}
