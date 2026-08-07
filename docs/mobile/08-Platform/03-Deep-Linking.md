---
title: Deep Linking & Universal Links Architecture
module: Platform
platform: Flutter
backend: Identity Platform
version: 1.0
status: Draft
owner: Mobile Architecture Team
---

# 🔗 Deep Linking & Universal Links

> Defines the complete deep linking architecture for Sports Gurukul, enabling secure navigation from push notifications, emails, QR codes, SMS, websites, and third-party applications directly into the mobile application.

---

# Table of Contents

1. Overview
2. Business Goals
3. Deep Link Types
4. URL Structure
5. Navigation Architecture
6. Authentication Flow
7. Notification Deep Links
8. QR Code Deep Links
9. Email Links
10. Universal Links
11. Dynamic Links
12. Security
13. Analytics
14. Testing
15. Acceptance Criteria

---

# 1. Overview

Deep Linking enables users to open specific content directly inside the application.

Examples

- Open Tournament
- Open Payment Invoice
- Open Training Session
- Open Chat
- Open Event
- Open Medical Record
- Open Achievement

---

# 2. Business Goals

Increase

- User Engagement
- Conversion Rate
- Payment Completion
- Tournament Registration
- Notification Open Rate

Reduce

- Navigation Steps
- User Friction
- Drop-offs

---

# 3. Supported Link Types

Supports

- App Links (Android)
- Universal Links (iOS)
- Custom URL Scheme
- QR Code Links
- Email Links
- Push Notification Links
- Web Redirects

---

# 4. URL Structure

Base URL

```
https://app.sportsgurukul.com
```

Examples

```
/training/TRN001

/tournaments/T001

/events/E001

/payments/INV123

/chat/C987

/profile

/documents/D100

/achievements

/attendance

/medical

/settings
```

---

# 5. Custom Scheme

```
sportsgurukul://training/TRN001

sportsgurukul://payment/INV100

sportsgurukul://event/E500
```

---

# 6. Navigation Flow

```text
Deep Link

↓

App Open

↓

Authentication

↓

Permission Validation

↓

Fetch Data

↓

Navigate

↓

Target Screen
```

---

# 7. Authentication Rules

User Logged In

↓

Open Screen

User Logged Out

↓

Login

↓

Redirect

↓

Original Screen

---

# 8. Notification Deep Links

Examples

Training Reminder

↓

Training Details

Payment Due

↓

Invoice Screen

Tournament Reminder

↓

Tournament Details

Coach Message

↓

Chat Screen

Medical Reminder

↓

Medical Dashboard

Achievement

↓

Achievement Screen

---

# 9. QR Code Navigation

QR Codes may open

- Event Check-In
- Tournament Entry
- Academy Gate Pass
- Attendance
- Digital Athlete Card
- Certificate Verification

Example

```
https://app.sportsgurukul.com/event/checkin/EV1001
```

---

# 10. Email Links

Examples

Verify Email

↓

Email Verification Screen

Reset Password

↓

Password Reset

Tournament Invitation

↓

Tournament Details

Receipt Download

↓

Invoice

---

# 11. Universal Links

Android

```
assetlinks.json
```

Hosted at

```
https://app.sportsgurukul.com/.well-known/
```

---

iOS

```
apple-app-site-association
```

Hosted at

```
https://app.sportsgurukul.com/.well-known/
```

---

# 12. Route Mapping

| Link              | Screen                |
| ----------------- | --------------------- |
| /training/{id}    | Training Details      |
| /attendance       | Attendance            |
| /performance      | Performance Dashboard |
| /tournaments/{id} | Tournament Details    |
| /events/{id}      | Event Details         |
| /payments/{id}    | Invoice               |
| /wallet           | Wallet                |
| /chat/{id}        | Chat                  |
| /documents/{id}   | Document Viewer       |
| /medical          | Medical Dashboard     |
| /profile          | Profile               |
| /settings         | Settings              |

---

# 13. Flutter Architecture

Packages

```
go_router

app_links

firebase_messaging

flutter_local_notifications
```

Navigation

```
App Start

↓

DeepLinkService

↓

AuthenticationGuard

↓

PermissionGuard

↓

Router

↓

Screen
```

---

# 14. Deep Link Service

Responsibilities

- Parse URL
- Validate Route
- Authenticate User
- Resolve Parameters
- Navigate
- Track Analytics

---

# 15. Error Handling

Invalid Link

↓

404 Screen

Expired Link

↓

Expired Message

Unauthorized

↓

Login

↓

Retry

Deleted Resource

↓

Resource Not Found

---

# 16. Security

JWT Authentication

Signed URLs (Sensitive Resources)

Token Expiration

Permission Validation

Replay Protection

Audit Logging

---

# 17. Analytics

Track

```
deep_link_received

deep_link_opened

notification_opened

qr_code_scanned

email_link_clicked

route_redirected

invalid_link

deep_link_failed
```

---

# 18. Performance Goals

Route Parsing

<20 ms

Authentication Check

<100 ms

Navigation

<300 ms

Cold Start

<2 seconds

Warm Start

<500 ms

---

# 19. Accessibility

Supports

- Screen Reader
- VoiceOver
- TalkBack
- Keyboard Navigation

Deep-linked screens maintain accessibility focus.

---

# 20. Acceptance Criteria

✓ Universal Links configured

✓ Android App Links configured

✓ Custom URL schemes supported

✓ Authentication redirect works

✓ Notification deep links functional

✓ QR code navigation supported

✓ Invalid links handled gracefully

✓ Analytics integrated

✓ Secure routing implemented

✓ Responsive navigation

---

# Related Backend Modules

Identity Platform

Notification Platform

Tournament Platform

Training Platform

Finance Platform

Communication Platform

Document Platform

Medical Platform

Analytics Platform

---

# Future Enhancements

- Marketing campaign attribution
- Deferred deep linking
- Referral program links
- Smart QR campaigns
- Dynamic link personalization
- Branch.io integration (optional)
- Cross-platform link analytics

---

# Next Documents

04-Analytics-&-Telemetry.md

05-Performance-Optimization.md

06-Security-&-Compliance.md

07-Localization.md

08-UI-Component-Library.md

09-Testing-Strategy.md

10-CI-CD-&-Release-Management.md

---

**End of Document**
