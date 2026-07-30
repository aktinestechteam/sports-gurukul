namespace SportsGurukul.Platform.Communication.Configuration;

public class CommunicationOptions
{
    public DeliveryOptions Delivery { get; set; } = new();
    public RetryOptions Retry { get; set; } = new();
    public QueueOptions Queue { get; set; } = new();
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
    public TemplateEngineOptions TemplateEngine { get; set; } = new();
    public ProviderOptions Providers { get; set; } = new();
    public ObservabilityOptions Observability { get; set; } = new();
    public SecurityOptions Security { get; set; } = new();
}

public class DeliveryOptions
{
    public int MaxConcurrentDeliveries { get; set; } = 10;
    public int BulkBatchSize { get; set; } = 100;
    public int ThrottleDelayMs { get; set; } = 50;
    public bool FailoverEnabled { get; set; } = true;
    public int FailoverTimeoutSeconds { get; set; } = 2;
    public bool DeadLetterEnabled { get; set; } = true;
}

public class RetryOptions
{
    public int MaxRetries { get; set; } = 3;
    public int BaseDelayMs { get; set; } = 1000;
    public int MaxDelayMs { get; set; } = 30000;
    public double BackoffMultiplier { get; set; } = 2.0;
    public bool JitterEnabled { get; set; } = true;
    public int JitterMaxMs { get; set; } = 500;
}

public class QueueOptions
{
    public int PollingIntervalMs { get; set; } = 1000;
    public int BatchSize { get; set; } = 50;
    public int MaxConcurrentProcessors { get; set; } = 4;
    public int StaleLockTimeoutMinutes { get; set; } = 30;
    public bool ScheduledDeliveryEnabled { get; set; } = true;
    public int ScheduledPollingIntervalMs { get; set; } = 15000;
}

public class CircuitBreakerOptions
{
    public int FailureThreshold { get; set; } = 5;
    public int SuccessThreshold { get; set; } = 2;
    public int OpenDurationSeconds { get; set; } = 30;
    public int HalfOpenMaxAttempts { get; set; } = 1;
    public bool Enabled { get; set; } = true;
}

public class TemplateEngineOptions
{
    public string DefaultEngine { get; set; } = "Handlebars";
    public bool EnableLocalization { get; set; } = true;
    public string DefaultLocale { get; set; } = "en";
    public bool StrictMode { get; set; } = false;
    public bool CacheCompiledTemplates { get; set; } = true;
    public int CacheMaxSize { get; set; } = 500;
}

public class ProviderOptions
{
    public Dictionary<string, ProviderConfig> Providers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ProviderConfig
{
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ObservabilityOptions
{
    public bool MetricsEnabled { get; set; } = true;
    public bool HealthChecksEnabled { get; set; } = true;
    public int HealthCheckIntervalSeconds { get; set; } = 60;
}

public class SecurityOptions
{
    public bool AuditLoggingEnabled { get; set; } = true;
    public bool DataMaskingEnabled { get; set; } = true;
    public bool WebhookSignatureValidationEnabled { get; set; } = true;
    public string DefaultWebhookSecret { get; set; } = string.Empty;
}
