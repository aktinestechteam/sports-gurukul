# Notification Domain — Sports Gurukul

## Overview

The Notification Domain is a platform module that provides a unified, multi-channel communication system. It is designed to be reused by all other platform modules — Authentication, Athlete, Coach, Academy, Training, Tournament, Event, Finance, and future AI services.

## Entity Relationships

```
NotificationChannel (1) ──< (N) NotificationProvider
NotificationChannel (1) ──< (N) Notification
NotificationChannel (1) ──< (N) NotificationPreference
NotificationChannel (1) ──< (N) NotificationSubscription

NotificationTemplate (1) ──< (N) TemplateVersion
NotificationTemplate (1) ──< (N) TemplateVariable
NotificationTemplate (1) ──< (N) Notification

Notification (1) ──< (N) NotificationRecipient
Notification (1) ──< (N) NotificationDelivery
Notification (1) ──< (N) NotificationAttachment
Notification (1) ── (0..1) NotificationSchedule
Notification (1) ── (0..1) NotificationQueue
Notification (N) >── (1) NotificationBatch
Notification (N) >── (1) NotificationCampaign

NotificationDelivery (1) ──< (N) NotificationRetry

NotificationRecipient (N) >── (1) User

NotificationPreference (N) >── (1) User
NotificationSubscription (N) >── (1) User
```

## Aggregate Boundaries

| Aggregate Root            | Entities                                      | Description                                  |
|---------------------------|-----------------------------------------------|----------------------------------------------|
| Notification              | NotificationRecipient, NotificationDelivery, NotificationAttachment, NotificationSchedule, NotificationQueue | Core notification aggregate                  |
| NotificationTemplate      | TemplateVersion, TemplateVariable             | Template with versioned content & variables  |
| NotificationChannel       | NotificationProvider                          | Channel configuration and provider mapping   |
| NotificationBatch         | Notification                                  | Batch grouping with aggregated stats         |
| NotificationCampaign      | Notification                                  | Campaign grouping with scheduling & criteria |
| NotificationPreference    | —                                             | User notification preferences per channel    |
| NotificationSubscription  | —                                             | User subscriptions to entity events          |
| NotificationAudit         | —                                             | Audit trail for notification operations      |
| NotificationEvent         | —                                             | Domain event log for async processing        |
| NotificationRetry         | —                                             | Retry attempts for failed deliveries         |

## Supported Channels

- Email
- SMS
- WhatsApp
- Push Notification
- In-App Notification
- Webhook
- *Extensible via NotificationChannel/NotificationProvider entities*

## Notification Status Flow

```
Draft → Queued → Scheduled → Sending → Sent → Delivered → Read
                                        → Failed → (Retry) → Queued
                                        → Cancelled
                                        → Expired
```

## Priority Levels

- Low
- Normal (default)
- High
- Critical

## Extension Points

1. **Channels** — Add new `NotificationChannel` entries to support additional channels (e.g., Telegram, Slack).
2. **Providers** — Register new `NotificationProvider` per channel. Each provider stores JSON configuration.
3. **Templates** — Create `NotificationTemplate` with `{{variable}}` placeholders. Variables are resolved at send time.
4. **Retry Policies** — Configured via `MaxAttempts` and `NextAttemptAt` on `NotificationQueue`.
5. **Scheduling** — Use `NotificationSchedule` with optional `RecurrenceRule` (CRON) for recurring notifications.
6. **Campaigns** — Define `NotificationCampaign` with `TargetCriteria` (JSON filter) for segmented sends.
7. **Event System** — `NotificationEvent` stores domain events for outbox-style async processing.
8. **Audit Trail** — `NotificationAudit` captures all state changes for compliance and debugging.

## Repository Interfaces

| Interface             | Methods                                                                 |
|-----------------------|-------------------------------------------------------------------------|
| INotificationRepository | GetByIdWithDetails, GetByStatus, GetByPriority, GetByBatch, GetByCampaign, GetByUser, GetPending, GetScheduledDue |
| ITemplateRepository     | GetByName, GetWithVersions, GetByChannel, GetActiveTemplates           |
| IPreferenceRepository   | GetByUser, GetByUserAndChannel, IsChannelEnabled                       |
| IQueueRepository        | GetPendingItems, GetByStatus, GetByPriority, GetStaleLocks, GetByNotification |
| IDeliveryRepository     | GetByProviderMessageId, GetByNotification, GetByStatus, GetFailedDeliveries |
| IAuditRepository        | GetByEntity, GetByAction, GetByDateRange                               |

## Platform Services Reused

- **Identity Platform** — User (`UserId`) for recipient and preference lookups
- **Audit Platform** — `NotificationAudit` for change tracking
- **Document Platform** — `NotificationAttachment.DocumentId` for linked documents
- **Reference Data Platform** — `NotificationChannel`, `NotificationProvider` as reference entities

## Files Created

### Domain Layer (`SportsGurukul.Domain`)
| File | Path |
|------|------|
| NotificationChannelType enum | `Enums/Notification/NotificationChannelType.cs` |
| NotificationPriority enum | `Enums/Notification/NotificationPriority.cs` |
| NotificationStatus enum | `Enums/Notification/NotificationStatus.cs` |
| Notification | `Entities/Notification/Notification.cs` |
| NotificationRecipient | `Entities/Notification/NotificationRecipient.cs` |
| NotificationTemplate | `Entities/Notification/NotificationTemplate.cs` |
| TemplateVersion | `Entities/Notification/TemplateVersion.cs` |
| TemplateVariable | `Entities/Notification/TemplateVariable.cs` |
| NotificationChannel | `Entities/Notification/NotificationChannel.cs` |
| NotificationProvider | `Entities/Notification/NotificationProvider.cs` |
| NotificationPreference | `Entities/Notification/NotificationPreference.cs` |
| NotificationSubscription | `Entities/Notification/NotificationSubscription.cs` |
| NotificationSchedule | `Entities/Notification/NotificationSchedule.cs` |
| NotificationQueue | `Entities/Notification/NotificationQueue.cs` |
| NotificationAttachment | `Entities/Notification/NotificationAttachment.cs` |
| NotificationDelivery | `Entities/Notification/NotificationDelivery.cs` |
| NotificationRetry | `Entities/Notification/NotificationRetry.cs` |
| NotificationBatch | `Entities/Notification/NotificationBatch.cs` |
| NotificationCampaign | `Entities/Notification/NotificationCampaign.cs` |
| NotificationEvent | `Entities/Notification/NotificationEvent.cs` |
| NotificationAudit | `Entities/Notification/NotificationAudit.cs` |

### Application Layer (`SportsGurukul.Application`)
| File | Path |
|------|------|
| INotificationRepository | `Common/Interfaces/Notification/INotificationRepository.cs` |
| ITemplateRepository | `Common/Interfaces/Notification/ITemplateRepository.cs` |
| IPreferenceRepository | `Common/Interfaces/Notification/IPreferenceRepository.cs` |
| IQueueRepository | `Common/Interfaces/Notification/IQueueRepository.cs` |
| IDeliveryRepository | `Common/Interfaces/Notification/IDeliveryRepository.cs` |
| IAuditRepository | `Common/Interfaces/Notification/IAuditRepository.cs` |

### Infrastructure Layer (`SportsGurukul.Infrastructure`)
| File | Path |
|------|------|
| NotificationConfiguration | `Persistence/Configurations/Notification/NotificationConfiguration.cs` |
| NotificationRecipientConfiguration | `Persistence/Configurations/Notification/NotificationRecipientConfiguration.cs` |
| NotificationTemplateConfiguration | `Persistence/Configurations/Notification/NotificationTemplateConfiguration.cs` |
| TemplateVersionConfiguration | `Persistence/Configurations/Notification/TemplateVersionConfiguration.cs` |
| TemplateVariableConfiguration | `Persistence/Configurations/Notification/TemplateVariableConfiguration.cs` |
| NotificationChannelConfiguration | `Persistence/Configurations/Notification/NotificationChannelConfiguration.cs` |
| NotificationProviderConfiguration | `Persistence/Configurations/Notification/NotificationProviderConfiguration.cs` |
| NotificationPreferenceConfiguration | `Persistence/Configurations/Notification/NotificationPreferenceConfiguration.cs` |
| NotificationSubscriptionConfiguration | `Persistence/Configurations/Notification/NotificationSubscriptionConfiguration.cs` |
| NotificationScheduleConfiguration | `Persistence/Configurations/Notification/NotificationScheduleConfiguration.cs` |
| NotificationQueueConfiguration | `Persistence/Configurations/Notification/NotificationQueueConfiguration.cs` |
| NotificationAttachmentConfiguration | `Persistence/Configurations/Notification/NotificationAttachmentConfiguration.cs` |
| NotificationDeliveryConfiguration | `Persistence/Configurations/Notification/NotificationDeliveryConfiguration.cs` |
| NotificationRetryConfiguration | `Persistence/Configurations/Notification/NotificationRetryConfiguration.cs` |
| NotificationBatchConfiguration | `Persistence/Configurations/Notification/NotificationBatchConfiguration.cs` |
| NotificationCampaignConfiguration | `Persistence/Configurations/Notification/NotificationCampaignConfiguration.cs` |
| NotificationEventConfiguration | `Persistence/Configurations/Notification/NotificationEventConfiguration.cs` |
| NotificationAuditConfiguration | `Persistence/Configurations/Notification/NotificationAuditConfiguration.cs` |
| NotificationRepository | `Persistence/Repositories/Notification/NotificationRepository.cs` |
| TemplateRepository | `Persistence/Repositories/Notification/TemplateRepository.cs` |
| PreferenceRepository | `Persistence/Repositories/Notification/PreferenceRepository.cs` |
| QueueRepository | `Persistence/Repositories/Notification/QueueRepository.cs` |
| DeliveryRepository | `Persistence/Repositories/Notification/DeliveryRepository.cs` |
| AuditRepository | `Persistence/Repositories/Notification/AuditRepository.cs` |
| Migration | `Persistence/Migrations/20260730084525_AddNotificationDomain.cs` |

### Modified Files
| File | Change |
|------|--------|
| `ApplicationDbContext.cs` | Added 18 DbSet properties for notification entities |
| `IApplicationDbContext.cs` | Added 18 DbSet properties for notification entities |
| `DependencyInjection.cs` | Added 6 repository DI registrations |
