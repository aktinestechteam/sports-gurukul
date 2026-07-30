# Operational Runbooks — Communication Platform

> **Module:** CommunicationPlatform
> **Last Updated:** 2026-07-30

---

## Table of Contents

1. [Health Check Monitoring](#1-health-check-monitoring)
2. [Notification Delivery Failure Investigation](#2-notification-delivery-failure-investigation)
3. [Queue Backlog Resolution](#3-queue-backlog-resolution)
4. [Template Rendering Issues](#4-template-rendering-issues)
5. [Campaign Execution Failure](#5-campaign-execution-failure)
6. [Database Connection Issues](#6-database-connection-issues)
7. [Rate Limiting Breach](#7-rate-limiting-breach)
8. [Secrets Rotation](#8-secrets-rotation)

---

## 1. Health Check Monitoring

### Endpoint
```
GET /health
```

### Expected Response
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456"
}
```

### Runbook Steps

**If health check returns Unhealthy:**

1. Check application logs for error messages
2. Verify database connectivity from the application server
3. Verify dependent services (SMTP server, SMS gateway, push notification services)
4. Check if the application is running with correct environment variables
5. Restart the application pod if necessary

**If health check times out:**

1. Check CPU/memory usage on the host
2. Check for deadlock or long-running database queries
3. Review recent deployment changes
4. Consider scaling up the instance

### Alert Threshold
- PagerDuty if `/health` returns non-200 for 2 consecutive checks (1 minute)
- Create ticket if response time exceeds 5 seconds for 5 consecutive checks

---

## 2. Notification Delivery Failure Investigation

### Symptoms
- High rate of `Failed` status in notification records
- Users reporting not receiving notifications
- Delivery statistics showing elevated failure rates

### Runbook Steps

1. **Query failed notifications by channel:**
   ```
   GET /api/v1/delivery?notificationId={id}
   GET /api/v1/delivery/statistics?fromDate=...&toDate=...
   ```

2. **Check provider health:**
   The `ProviderHealthChecker` background service polls all registered providers periodically. Check its logs for provider-specific failures.

3. **Check each provider:**
   - **Email (SMTP/SendGrid/SES):** Verify SMTP server connectivity, API key validity, sender reputation
   - **SMS (Twilio/MSG91):** Verify API credentials, account balance, region availability
   - **Push (Firebase/APNs):** Verify FCM/APNs credentials, device token validity
   - **WhatsApp (Meta/Twilio):** Verify WhatsApp Business account status, template approval

4. **Reprocess failed notifications:**
   ```
   POST /api/v1/queue/reprocess
   { "notificationIds": ["guid1", "guid2", ...] }
   ```

5. **Escalate if:** Failure rate > 10% for more than 15 minutes

### Common Fixes

| Issue | Fix |
|---|---|
| SMTP server unreachable | Check firewall rules, DNS resolution, TLS certificate |
| API key expired | Rotate provider API key, update in Secrets Manager |
| Provider rate limit hit | Implement client-side throttling, check provider limits |
| Invalid recipient address | Log invalid addresses, return clear error to calling service |
| Template rendering error | Verify template variables match expected format |

---

## 3. Queue Backlog Resolution

### Symptoms
- Increasing queue depth in `NotificationQueue` table
- Delayed notification delivery
- High CPU/memory on queue processor instances

### Runbook Steps

1. **Check queue depth:**
   Query `NotificationQueue` table for items with status `Queued` ordered by `QueuedAt`.

2. **Check for stuck items:**
   Look for items with `LockExpiresAt` in the past and status `Processing`. The `DeadLetterQueueHandler` background service automatically detects and releases stale locks.

3. **Scale queue processors:**
   If queue depth exceeds 10,000 items, scale out the `QueueBackgroundProcessor` instances.

4. **Check bulk delivery throttling:**
   The `BulkDeliveryService` uses configurable throttling. Check `CommunicationOptions.Delivery.BatchSize` and `CommunicationOptions.Delivery.ThrottleIntervalMs`.

5. **Emergency purge (last resort):**
   If queue is stuck and not recovering, mark items as `Failed` and re-queue through the reprocess endpoint.

### Batch Operations

| Action | Endpoint | Notes |
|---|---|---|
| View queue | `GET /api/v1/queue` | Lists all items |
| View failed | `GET /api/v1/queue/failed` | Lists failed items only |
| Reprocess | `POST /api/v1/queue/reprocess` | Batch reprocess by IDs |

---

## 4. Template Rendering Issues

### Symptoms
- Notifications sent with incorrect content
- Template variables not resolved
- "Template not found" errors in notification creation

### Runbook Steps

1. **Verify template exists and is published:**
   ```
   GET /api/v1/templates/{id}
   GET /api/v1/templates/{id}/versions
   ```

2. **Check template content:**
   Verify the template body uses correct syntax for the configured template engine (Handlebars or Liquid).

3. **Check variable resolution:**
   The `VariableResolver` uses global variable providers. Verify that variable names match the template exactly.

4. **Re-publish template if needed:**
   ```
   POST /api/v1/templates/{id}/publish
   ```

5. **Create new version with fix:**
   Update template content, re-publish to create a new version.

### Known Template Engine Behaviors

| Engine | Syntax | Escaping |
|---|---|---|
| Handlebars | `{{var}}`, `{{#if}}`, `{{#each}}` | HTML-escaped by default, use `{{{var}}}` for raw |
| Liquid | `{{var}}`, `{% if %}`, `{% for %}` | Auto-escape configurable |

---

## 5. Campaign Execution Failure

### Symptoms
- Campaign stuck in `Scheduled` status past `ScheduledAt` time
- Campaign notifications not being created
- Campaign status shows unexpected values

### Runbook Steps

1. **Check campaign status:**
   ```
   GET /api/v1/campaigns/{id}
   ```

2. **Verify scheduled time:**
   Check `ScheduledAt` field is in the past and correct timezone.

3. **Check `ScheduledDeliveryService`:**
   This background service processes due campaigns every polling interval. Check its logs for errors.

4. **Manually trigger campaign:**
   Use the schedule endpoint to re-schedule the campaign immediately.

5. **Cancel and recreate if stuck:**
   ```
   POST /api/v1/campaigns/{id}/cancel
   ```

### Campaign Status States

```
Draft → Scheduled → Active → Completed
                     ↓
                  Paused → Active  (resume workflow)
                     ↓
                  Cancelled  (any state)
```

---

## 6. Database Connection Issues

### Symptoms
- `Cannot open database` errors in logs
- Health check failing with database timeout
- All notification operations returning 500 errors

### Runbook Steps

1. **Verify connection string:**
   Check that the `ConnectionStrings:Default` environment variable is correctly set in the deployment configuration.

2. **Check database server status:**
   - Verify PostgreSQL server is running
   - Check connection pool exhaustion (`max_pool_size` in connection string)
   - Check for active locks on notification tables

3. **Verify migrations are applied:**
   ```
   SELECT * FROM __EFMigrationsHistory;
   ```
   Ensure all migration entries exist.

4. **Scale database:**
   If connection pool is exhausted, increase `max_pool_size` or add read replicas.

### Connection String Template
```
Host={host};Database=sportsgurukul;Username={user};Password={password};MaxPoolSize=100;Timeout=15;CommandTimeout=30;
```

---

## 7. Rate Limiting Breach

### Symptoms
- Users receiving `429 Too Many Requests` responses
- Application logs showing rate limit exceeded messages

### Runbook Steps

1. **Check current rate limit configuration:**
   Three policies are configured:
   - `"auth"`: 10 requests per minute, no queue
   - `"sensitive"`: 5 requests per 5 minutes, no queue
   - `"default"`: 100 requests per minute, queue limit 10

2. **Determine if breach is legitimate abuse:**
   Check IP address, user ID, and endpoint pattern in logs.

3. **Temporarily adjust limits if false positive:**
   Update `RateLimiterOptions` in `Program.cs` and redeploy if needed.

4. **Add offending IP/user to permanent block list:**
   Use firewall rules or application-level block list.

### Rate Limiting Rules Applied

| Controller | Policy | Limit |
|---|---|---|
| `NotificationsController` | default | 100 req/min |
| `CampaignsController` | default | 100 req/min |
| `TemplatesController` | default | 100 req/min |
| `DeliveryController` | default | 100 req/min |
| `QueueController` | default | 100 req/min |
| `PreferencesController` | default | 100 req/min |
| `AuthController` (sensitive endpoints) | sensitive | 5 req/5 min |

---

## 8. Secrets Rotation

### Scheduled Rotation

| Secret | Rotation Frequency | Impact if Compromised |
|---|---|---|
| JWT Signing Key | Every 90 days | Token forgery |
| Database Password | Every 90 days | Full data access |
| SMTP Password | Every 90 days | Email spoofing |
| Provider API Keys | Every 180 days | Provider access |

### Rotation Procedure

1. **Generate new secret value**
2. **Update in Secrets Manager** (Azure Key Vault / AWS Secrets Manager)
3. **Update environment variable** or restart application to pick up new value
4. **Verify** application functions correctly with new secret
5. **Revoke old secret** after 24-hour grace period

### Emergency Rotation

If a secret is compromised:
1. Immediately revoke the compromised secret
2. Generate and deploy new secret
3. Notify affected users if personal data may be exposed
4. Audit access logs for unauthorized access
