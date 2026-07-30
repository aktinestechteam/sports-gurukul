# Communication Platform — Notification Delivery Engine

## Architecture

```
┌──────────────────────────────────────────────────────────┐
│                   Communication Platform                  │
│  ┌────────────────────────────────────────────────────┐  │
│  │              Delivery Engine                         │  │
│  │  ┌─────────────┐  ┌──────────┐  ┌───────────────┐  │  │
│  │  │ Notification │  │  Retry   │  │   Circuit     │  │  │
│  │  │  Dispatcher  │  │  Engine  │  │   Breaker     │  │  │
│  │  └──────┬───────┘  └──────────┘  └───────────────┘  │  │
│  │         │                                            │  │
│  │  ┌──────▼───────┐  ┌──────────┐  ┌───────────────┐  │  │
│  │  │  Priority    │  │ Delivery │  │  Dead Letter   │  │  │
│  │  │  Queue Proc  │  │  Tracker │  │  Queue Handler │  │  │
│  │  └──────────────┘  └──────────┘  └───────────────┘  │  │
│  └────────────────────────────────────────────────────┘  │
│                                                           │
│  ┌────────────────────────────────────────────────────┐  │
│  │              Queue Processing                       │  │
│  │  ┌──────────────┐  ┌──────────────────┐  ┌──────┐  │  │
│  │  │   Queue      │  │  Background      │  │Sched │  │  │
│  │  │   Service    │  │  Processor       │  │Deliv │  │  │
│  │  └──────────────┘  └──────────────────┘  └──────┘  │  │
│  └────────────────────────────────────────────────────┘  │
│                                                           │
│  ┌────────────────────────────────────────────────────┐  │
│  │                Provider Layer                       │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────┐  │  │
│  │  │ Factory  │ │  Email   │ │   SMS    │ │WhatsApp│  │  │
│  │  │          │ │ Providers│ │ Providers│ │Prov.   │  │  │
│  │  └──────────┘ └──────────┘ └──────────┘ └───────┘  │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐            │  │
│  │  │   Push   │ │ In-App   │ │ Webhook  │            │  │
│  │  │ Providers│ │ Provider │ │ Provider │            │  │
│  │  └──────────┘ └──────────┘ └──────────┘            │  │
│  └────────────────────────────────────────────────────┘  │
│                                                           │
│  ┌────────────────────────────────────────────────────┐  │
│  │          Template Rendering Engine                  │  │
│  │  ┌──────────┐ ┌──────────┐ ┌────────┐ ┌────────┐  │  │
│  │  │Handlebars│ │  Liquid  │ │Variable│ │Localiz │  │  │
│  │  │ Engine   │ │  Engine  │ │Resolver│ │ Engine │  │  │
│  │  └──────────┘ └──────────┘ └────────┘ └────────┘  │  │
│  └────────────────────────────────────────────────────┘  │
│                                                           │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │  │
│  │ Webhook  │ │ Security │ │Observab.│ │ Config   │  │  │
│  │ Process. │ │(Mask,Aud)│ │(Metrics)│ │ Options  │  │  │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │  │
└──────────────────────────────────────────────────────────┘
```

## Project Structure

```
SportsGurukul.Platform.Communication/
├── Abstractions/
│   ├── INotificationProvider.cs        # Base provider interface + models
│   ├── IEmailProvider.cs               # Email channel marker
│   ├── ISmsProvider.cs                 # SMS channel marker
│   ├── IWhatsAppProvider.cs            # WhatsApp channel marker
│   ├── IPushProvider.cs                # Push notification channel marker
│   ├── IInAppProvider.cs               # In-app notification channel marker
│   ├── IWebhookProvider.cs             # Webhook channel marker
│   └── INotificationProviderFactory.cs # Provider selection factory
├── Providers/
│   ├── ProviderBase.cs                 # Abstract base with simulation helpers
│   ├── NotificationProviderFactory.cs  # Config-driven provider selector
│   ├── Email/
│   │   ├── SmtpEmailProvider.cs        # SMTP (stub)
│   │   ├── SendGridEmailProvider.cs    # SendGrid (stub)
│   │   ├── AmazonSesEmailProvider.cs   # Amazon SES (stub)
│   │   └── AzureCommunicationEmailProvider.cs  # Azure ACS (stub)
│   ├── Sms/
│   │   ├── TwilioSmsProvider.cs        # Twilio SMS (stub)
│   │   ├── Msg91SmsProvider.cs         # MSG91 (stub)
│   │   └── TextLocalSmsProvider.cs     # TextLocal (stub)
│   ├── WhatsApp/
│   │   ├── MetaWhatsAppProvider.cs     # Meta WhatsApp Cloud API (stub)
│   │   └── TwilioWhatsAppProvider.cs   # Twilio WhatsApp (stub)
│   ├── Push/
│   │   ├── FirebasePushProvider.cs     # Firebase Cloud Messaging (stub)
│   │   └── ApplePushProvider.cs        # APNs (stub)
│   ├── WebhookProvider.cs             # Generic HTTP webhook (stub)
│   └── InAppProvider.cs               # SignalR in-app (stub)
├── Delivery/
│   ├── NotificationDispatcher.cs       # Core dispatcher (implements INotificationDispatcher)
│   ├── RetryEngine.cs                  # Exponential backoff, jitter, max retries
│   ├── CircuitBreaker.cs               # Circuit breaker (Closed/Open/HalfOpen)
│   ├── DeadLetterQueueHandler.cs       # Stale lock detection and cleanup
│   ├── DeliveryTracker.cs              # Delivery attempt recording
│   └── PriorityQueueProcessor.cs       # Queue item processor with priority ordering
├── Queue/
│   ├── QueueService.cs                 # Queue management (implements IQueueService)
│   ├── QueueBackgroundProcessor.cs     # Background queue polling (HostedService)
│   ├── ScheduledDeliveryService.cs     # Due notification processor (HostedService)
│   └── BulkDeliveryService.cs          # Batch delivery with throttling
├── Rendering/
│   ├── ITemplateEngine.cs              # Template engine abstraction
│   ├── TemplateRenderer.cs             # Renderer orchestrator (implements ITemplateRenderer)
│   ├── HandlebarsTemplateEngine.cs     # Handlebars syntax ({{var}}, {{#if}}, {{#each}})
│   ├── LiquidTemplateEngine.cs         # Liquid syntax ({{var}}, {% if %}, {% for %})
│   ├── VariableResolver.cs             # Variable resolution with global providers
│   ├── LocalizedTemplateEngine.cs      # Multi-locale template translation
│   └── TemplateCache.cs                # LRU compiled template cache
├── Webhook/
│   ├── WebhookDeliveryService.cs       # HTTP webhook delivery to callback URLs
│   └── WebhookSignatureValidator.cs    # HMAC-SHA256 signature generation/validation
├── Security/
│   ├── DataMasker.cs                   # Email/phone/sensitive data masking
│   ├── DeliveryAuditLogger.cs          # Audit trail for dispatch actions
│   └── SecretsManager.cs               # Config/env-based secret resolution
├── Observability/
│   ├── DeliveryMetricsCollector.cs     # Per-channel delivery/retry/latency metrics
│   ├── ProviderHealthChecker.cs        # Periodic provider health checks (HostedService)
│   └── MetricsLoggingService.cs        # Periodic metrics summary logging (HostedService)
├── Configuration/
│   └── CommunicationOptions.cs         # All configuration POCOs
├── DependencyInjection.cs
└── SportsGurukul.Platform.Communication.csproj
```

## Provider Architecture

### Interface Hierarchy

```
INotificationProvider
├── IEmailProvider         → SmtpEmailProvider, SendGridEmailProvider, AmazonSesEmailProvider, AzureCommunicationEmailProvider
├── ISmsProvider           → TwilioSmsProvider, Msg91SmsProvider, TextLocalSmsProvider
├── IWhatsAppProvider     → MetaWhatsAppProvider, TwilioWhatsAppProvider
├── IPushProvider         → FirebasePushProvider, ApplePushProvider
├── IInAppProvider        → InAppProvider
└── IWebhookProvider      → WebhookProvider
```

### Provider Selection

`NotificationProviderFactory` selects the best provider for a channel:
1. Collects all `INotificationProvider` implementations registered in DI
2. Filters by `NotificationChannelType` matching the requested channel
3. Orders by configurable `Priority` (lower = preferred)
4. Returns the highest-priority available provider

### Failover

When `DeliveryOptions.FailoverEnabled` is true, the dispatcher tries all providers for a channel in priority order. If the first fails after retries, it falls back to the next.

## Delivery Flow

```
1. API Request → CreateNotificationCommand → NotificationService.CreateAsync()
2. NotificationService saves notification → IQueueService.EnqueueAsync()
3. QueueBackgroundProcessor polls → PriorityQueueProcessor.ProcessQueueItemsAsync()
4. PriorityQueueProcessor calls INotificationDispatcher.DispatchAsync()
5. NotificationDispatcher:
   a. Loads notification with recipients
   b. For each recipient:
      - Checks preferences (channel enabled, quiet hours, opt-out)
      - Renders template content via ITemplateRenderer
      - Creates delivery record
      - Gets provider via INotificationProviderFactory
      - Sends via RetryEngine (exponential backoff, circuit breaker)
      - Records delivery attempt via DeliveryTracker
      - Writes audit log
      - Emits metrics
   c. Updates notification status (Sent/Failed/Partial)
```

## Retry Policy

| Parameter | Default | Description |
|-----------|---------|-------------|
| MaxRetries | 3 | Maximum delivery attempts |
| BaseDelayMs | 1000 | Initial backoff delay |
| MaxDelayMs | 30000 | Maximum backoff delay |
| BackoffMultiplier | 2.0 | Exponential factor (1s → 2s → 4s ...) |
| JitterEnabled | true | Add random jitter to prevent thundering herd |
| JitterMaxMs | 500 | Maximum jitter added to delay |

### Circuit Breaker

| Parameter | Default | Description |
|-----------|---------|-------------|
| FailureThreshold | 5 | Consecutive failures to open circuit |
| SuccessThreshold | 2 | Successful attempts to close circuit |
| OpenDurationSeconds | 30 | Time before transitioning to Half-Open |
| HalfOpenMaxAttempts | 1 | Attempts allowed in Half-Open state |

States: `Closed` (normal) → `Open` (rejecting) → `HalfOpen` (probing) → `Closed`

## Template Rendering

### Supported Syntax

| Feature | Handlebars | Liquid |
|---------|-----------|--------|
| Variable | `{{name}}` | `{{name}}` |
| Nested | `{{user.name}}` | `{{user.name}}` |
| Default | `{{name\|default}}` | `{{name \| default: 'val'}}` |
| Conditional | `{{#if cond}}...{{/if}}` | `{% if cond %}...{% endif %}` |
| Loop | `{{#each items}}...{{/each}}` | `{% for item in items %}...{% endfor %}` |
| Filters | — | `{{name \| upcase}}` |
| Partial | `{{> partial}}` | — |

### Localization

Variable placeholders `{t welcome_message}` are replaced with locale-specific translations registered via `LocalizedTemplateEngine.RegisterTranslations()`.

## Security

### Data Masking
- `MaskEmail()` — masks local part (e.g., `j***n@email.com`)
- `MaskPhone()` — shows last 4 digits (e.g., `******1234`)
- `MaskSensitiveValue()` — generic masking with configurable visible chars
- `MaskJsonSensitiveFields()` — regex-based JSON field masking

### Audit Logging
- `DeliveryAuditLogger.LogDispatch()` — records all dispatch attempts with masked recipient addresses
- `DeliveryAuditLogger.LogQueueAction()` — records queue lifecycle events

### Webhook Signature Validation
- HMAC-SHA256 signature generation and validation
- Timestamp replay protection (configurable max age)
- Headers: `X-Webhook-Signature`, `X-Webhook-Timestamp`

### Secrets Management
- `SecretsManager` resolves credentials from multiple sources:
  1. `Communication:Secrets:{key}` configuration section
  2. `Communication:Providers:{name}:Settings:{key}`
  3. Environment variables prefixed `COMMUNICATION_`

## Observability

### Metrics (DeliveryMetricsCollector)

| Metric | Description |
|--------|-------------|
| TotalCount | Total delivery attempts |
| SuccessCount | Successful deliveries |
| FailureCount | Failed deliveries |
| RetryCount | Retry attempts |
| AverageDurationMs | Average delivery latency |
| MaxDurationMs | Maximum delivery latency |
| MinDurationMs | Minimum delivery latency |
| CurrentQueueDepth | Current queue depth |
| SuccessRate | Success percentage |

### Health Checks
- `ProviderHealthChecker` — periodic background health checks on all providers
- Configurable interval (default: 60s)

## Configuration (appsettings.json)

```json
{
  "Communication": {
    "Delivery": {
      "MaxConcurrentDeliveries": 10,
      "BulkBatchSize": 100,
      "ThrottleDelayMs": 50,
      "FailoverEnabled": true,
      "FailoverTimeoutSeconds": 2,
      "DeadLetterEnabled": true
    },
    "Retry": {
      "MaxRetries": 3,
      "BaseDelayMs": 1000,
      "MaxDelayMs": 30000,
      "BackoffMultiplier": 2.0,
      "JitterEnabled": true,
      "JitterMaxMs": 500
    },
    "Queue": {
      "PollingIntervalMs": 1000,
      "BatchSize": 50,
      "MaxConcurrentProcessors": 4,
      "StaleLockTimeoutMinutes": 30,
      "ScheduledDeliveryEnabled": true,
      "ScheduledPollingIntervalMs": 15000
    },
    "CircuitBreaker": {
      "FailureThreshold": 5,
      "SuccessThreshold": 2,
      "OpenDurationSeconds": 30,
      "HalfOpenMaxAttempts": 1,
      "Enabled": true
    },
    "TemplateEngine": {
      "DefaultEngine": "Handlebars",
      "EnableLocalization": true,
      "DefaultLocale": "en",
      "StrictMode": false,
      "CacheCompiledTemplates": true,
      "CacheMaxSize": 500
    },
    "Providers": {
      "SMTP": { "IsActive": true, "IsDefault": true, "Priority": 1, "Settings": {} },
      "SendGrid": { "IsActive": true, "Priority": 2, "Settings": {} },
      "TwilioSMS": { "IsActive": true, "Priority": 1, "Settings": {} },
      "MetaWhatsApp": { "IsActive": true, "Priority": 1, "Settings": {} }
    },
    "Observability": {
      "MetricsEnabled": true,
      "HealthChecksEnabled": true,
      "HealthCheckIntervalSeconds": 60
    },
    "Security": {
      "AuditLoggingEnabled": true,
      "DataMaskingEnabled": true,
      "WebhookSignatureValidationEnabled": true
    }
  }
}
```

## DI Registration

```csharp
// Program.cs
services.AddCommunicationPlatform(configuration);
// Or with custom options:
services.AddCommunicationPlatform(configuration, opts =>
{
    opts.Retry.MaxRetries = 5;
    opts.Retry.BaseDelayMs = 2000;
});
```

This registers all providers, delivery engine, queue processing, rendering, webhook, security, and observability services. It overrides the Application-layer stubs for `INotificationDispatcher`, `IQueueService`, and `ITemplateRenderer` with real implementations.

## Performance Targets

| Metric | Target |
|--------|--------|
| Single notification dispatch | <150ms |
| Bulk dispatch (10,000 notifications) | <1 hour |
| Provider failover | <2s |
| Queue polling latency | <1s |
| Template rendering | <10ms |
