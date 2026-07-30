using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Platform.Communication.Abstractions;
using SportsGurukul.Platform.Communication.Configuration;
using SportsGurukul.Platform.Communication.Delivery;
using SportsGurukul.Platform.Communication.Observability;
using SportsGurukul.Platform.Communication.Providers;
using SportsGurukul.Platform.Communication.Providers.Email;
using SportsGurukul.Platform.Communication.Providers.Push;
using SportsGurukul.Platform.Communication.Providers.Sms;
using SportsGurukul.Platform.Communication.Providers.WhatsApp;
using SportsGurukul.Platform.Communication.Queue;
using SportsGurukul.Platform.Communication.Rendering;
using SportsGurukul.Platform.Communication.Security;
using SportsGurukul.Platform.Communication.Webhook;

namespace SportsGurukul.Platform.Communication;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunicationPlatform(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CommunicationOptions>? configureOptions = null)
    {
        var optionsSection = configuration.GetSection("Communication");
        services.Configure<CommunicationOptions>(optionsSection);

        if (configureOptions is not null)
        {
            services.PostConfigure<CommunicationOptions>(opts => configureOptions(opts));
        }

        RegisterProviders(services);
        RegisterDeliveryEngine(services);
        RegisterQueueProcessing(services);
        RegisterRendering(services);
        RegisterWebhook(services);
        RegisterSecurity(services);
        RegisterObservability(services);

        return services;
    }

    private static void RegisterProviders(IServiceCollection services)
    {
        services.AddSingleton<IEmailProvider, SmtpEmailProvider>();
        services.AddSingleton<IEmailProvider, SendGridEmailProvider>();
        services.AddSingleton<IEmailProvider, AmazonSesEmailProvider>();
        services.AddSingleton<IEmailProvider, AzureCommunicationEmailProvider>();

        services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
        services.AddSingleton<ISmsProvider, Msg91SmsProvider>();
        services.AddSingleton<ISmsProvider, TextLocalSmsProvider>();

        services.AddSingleton<IWhatsAppProvider, MetaWhatsAppProvider>();
        services.AddSingleton<IWhatsAppProvider, TwilioWhatsAppProvider>();

        services.AddSingleton<IPushProvider, FirebasePushProvider>();
        services.AddSingleton<IPushProvider, ApplePushProvider>();

        services.AddSingleton<IWebhookProvider, WebhookProvider>();
        services.AddSingleton<IInAppProvider, InAppProvider>();

        services.AddSingleton<INotificationProvider>(sp =>
        {
            var providers = sp.GetServices<IEmailProvider>()
                .Cast<INotificationProvider>()
                .Concat(sp.GetServices<ISmsProvider>())
                .Concat(sp.GetServices<IWhatsAppProvider>())
                .Concat(sp.GetServices<IPushProvider>())
                .Concat(sp.GetServices<IWebhookProvider>())
                .Concat(sp.GetServices<IInAppProvider>())
                .ToList();

            var factory = new AggregateProviderBridge(providers);
            return factory;
        });

        services.AddSingleton<INotificationProviderFactory, NotificationProviderFactory>();
    }

    private static void RegisterDeliveryEngine(IServiceCollection services)
    {
        services.AddSingleton<CircuitBreaker>();
        services.AddSingleton<DeliveryTracker>();
        services.AddSingleton<RetryEngine>();
        services.AddSingleton<DeadLetterQueueHandler>();
        services.AddSingleton<PriorityQueueProcessor>();

        services.AddTransient<INotificationDispatcher, NotificationDispatcher>();
    }

    private static void RegisterQueueProcessing(IServiceCollection services)
    {
        services.AddTransient<IQueueService, QueueService>();
        services.AddSingleton<BulkDeliveryService>();

        services.AddHostedService<QueueBackgroundProcessor>();
        services.AddHostedService<ScheduledDeliveryService>();
    }

    private static void RegisterRendering(IServiceCollection services)
    {
        services.AddSingleton<HandlebarsTemplateEngine>();
        services.AddSingleton<LiquidTemplateEngine>();

        services.AddSingleton<ITemplateEngine>(sp =>
            sp.GetRequiredService<HandlebarsTemplateEngine>());

        services.AddSingleton<VariableResolver>();
        services.AddSingleton<LocalizedTemplateEngine>();
        services.AddSingleton<TemplateCache>();

        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
    }

    private static void RegisterWebhook(IServiceCollection services)
    {
        services.AddHttpClient<WebhookDeliveryService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<WebhookSignatureValidator>();
    }

    private static void RegisterSecurity(IServiceCollection services)
    {
        services.AddSingleton<DataMasker>();
        services.AddSingleton<DeliveryAuditLogger>();
        services.AddSingleton<SecretsManager>();
    }

    private static void RegisterObservability(IServiceCollection services)
    {
        services.AddSingleton<DeliveryMetricsCollector>();

        if (services.Any(s => s.ServiceType == typeof(DeliveryMetricsCollector)))
        {
        }

        services.AddHostedService<ProviderHealthChecker>();
        services.AddHostedService<MetricsLoggingService>();
    }

    private class AggregateProviderBridge : INotificationProvider
    {
        private readonly IReadOnlyList<INotificationProvider> _providers;
        public string Name => "Aggregate";
        public Domain.Enums.Notification.NotificationChannelType ChannelType
            => throw new NotSupportedException();
        public bool IsAvailable => true;

        public AggregateProviderBridge(IReadOnlyList<INotificationProvider> providers)
        {
            _providers = providers;
        }

        public Task<ProviderSendResult> SendAsync(ProviderMessage message, CancellationToken ct = default)
            => throw new NotSupportedException("Use INotificationProviderFactory instead");

        public Task<bool> HealthCheckAsync(CancellationToken ct = default)
            => Task.FromResult(true);
    }
}
