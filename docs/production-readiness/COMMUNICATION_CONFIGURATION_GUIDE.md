# Configuration Guide — Communication Platform

> **Module:** CommunicationPlatform
> **Last Updated:** 2026-07-30

---

## Environment Variables

All secrets and environment-specific settings should be provided via environment variables, not `appsettings.json`.

### Required Variables

| Variable | Description | Example | Required |
|---|---|---|---|
| `ConnectionStrings__Default` | PostgreSQL connection string | `Host=pg-prod;Database=sportsgurukul;Username=svc_notifications;Password=***` | Yes |
| `Jwt__SigningKey` | HMAC-SHA256 key for JWT tokens (min 32 chars) | A 256-bit key encoded as string | Yes |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` | Yes |

### Optional Variables

| Variable | Default | Description |
|---|---|---|
| `Jwt__Issuer` | `SportsGurukul` | JWT token issuer |
| `Jwt__Audience` | `SportsGurukul` | JWT token audience |
| `Jwt__AccessTokenExpirationMinutes` | `60` | Access token TTL |
| `Jwt__RefreshTokenExpirationDays` | `30` | Refresh token TTL |
| `Smtp__Host` | `smtp.gmail.com` | SMTP server hostname |
| `Smtp__Port` | `587` | SMTP server port |
| `Smtp__Username` | `""` | SMTP authentication username |
| `Smtp__Password` | `""` | SMTP authentication password |
| `Smtp__SenderEmail` | `noreply@sportsgurukul.com` | From address for outgoing email |
| `Smtp__SenderName` | `Sports Gurukul` | Display name for sender |
| `Smtp__EnableSsl` | `true` | Enable SSL for SMTP |
| `Smtp__UseMockSender` | `true` | Mock mode (logs instead of sending) |

---

## Communication Platform Options

When the Communication Platform library is wired in, the following options are available via the `Communication` config section:

```json
{
  "Communication": {
    "Delivery": {
      "BatchSize": 50,
      "ThrottleIntervalMs": 1000,
      "MaxConcurrentDeliveries": 10
    },
    "Retry": {
      "MaxRetries": 3,
      "BaseDelayMs": 1000,
      "MaxDelayMs": 30000,
      "JitterFactor": 0.2
    },
    "Queue": {
      "PollingIntervalMs": 5000,
      "MaxBatchSize": 100,
      "StaleLockTimeoutMinutes": 5
    },
    "CircuitBreaker": {
      "FailureThreshold": 5,
      "OpenDurationMs": 30000,
      "HalfOpenMaxAttempts": 3
    },
    "TemplateEngine": {
      "DefaultEngine": "Handlebars",
      "CacheSize": 100,
      "CacheSlidingExpirationMinutes": 60
    },
    "Providers": {
      "Email": {
        "DefaultProvider": "Smtp",
        "FallbackProvider": "SendGrid"
      },
      "Sms": {
        "DefaultProvider": "Twilio",
        "FallbackProvider": "Msg91"
      },
      "Push": {
        "DefaultProvider": "Firebase"
      }
    },
    "Observability": {
      "MetricsEnabled": true,
      "HealthCheckIntervalSeconds": 60,
      "MetricsLoggingIntervalMinutes": 5
    },
    "Security": {
      "MaskSensitiveData": true,
      "AuditEnabled": true,
      "AuditRetentionDays": 90
    }
  }
}
```

---

## Rate Limiting Configuration

Defined in `Program.cs` with three named policies:

| Policy | Window | Limit | Queue | Use Case |
|---|---|---|---|---|
| `"auth"` | 1 minute | 10 requests | No queuing | Authentication endpoints |
| `"sensitive"` | 5 minutes | 5 requests | No queuing | Password reset, email verification |
| `"default"` | 1 minute | 100 requests | Queue limit 10 | All other endpoints |

To adjust limits, modify the `RateLimiterOptions` in `Program.cs`:

```csharp
options.AddFixedWindowLimiter("default", opt =>
{
    opt.PermitLimit = 100;        // Requests per window
    opt.Window = TimeSpan.FromMinutes(1);
    opt.QueueLimit = 10;           // Max queued requests
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
});
```

---

## CORS Configuration

```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://app.sportsgurukul.com",
      "https://admin.sportsgurukul.com"
    ]
  }
}
```

If `Cors:AllowedOrigins` is not configured, falls back to `https://localhost:3000` and `https://localhost:5001`.

---

## Storage Configuration

```json
{
  "Storage": {
    "Provider": "Local",
    "BasePath": "uploads",
    "Azure": {
      "ConnectionString": "",
      "ContainerName": "attachments"
    },
    "S3": {
      "BucketName": "",
      "Region": "",
      "AccessKey": "",
      "SecretKey": ""
    }
  }
}
```

---

## Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "SportsGurukul": "Debug"
    }
  }
}
```

In production, consider using:
- Structured logging (Serilog with sinks for Elasticsearch, Application Insights, or Seq)
- Log levels: `Warning` for framework, `Information` for application, `Error` for exceptions only

---

## Health Check Endpoint

```
GET /health
```

Returns `200 OK` when the application is healthy. Configure monitoring to poll this endpoint every 30 seconds.

---

## Deployment Verification

After deployment, verify the following:

```powershell
# Health check
curl https://api.sportsgurukul.com/health

# Verify authentication
curl -H "Authorization: Bearer $TOKEN" https://api.sportsgurukul.com/api/v1/preferences

# Verify rate limiting
for ($i=0; $i -lt 120; $i++) { curl -H "Authorization: Bearer $TOKEN" https://api.sportsgurukul.com/api/v1/notifications }
# Expect 429 Too Many Requests after 100 requests

# Verify CORS
curl -H "Origin: https://app.sportsgurukul.com" -H "Access-Control-Request-Method: GET" -X OPTIONS https://api.sportsgurukul.com/api/v1/notifications
```
