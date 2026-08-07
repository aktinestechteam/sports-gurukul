---
title: Sports Gurukul Mobile Navigation Architecture
version: 1.0
status: Draft
owner: Solution Architecture Team
platform: Flutter
navigation: GoRouter
---

# 🧭 Mobile Navigation Architecture

> Defines routing, navigation flows, authentication guards, deep linking, role-based access, and screen hierarchy for all Sports Gurukul mobile applications.

---

# Table of Contents

1. Navigation Principles
2. Navigation Layers
3. Application Startup Flow
4. Authentication Flow
5. Athlete Navigation
6. Parent Navigation
7. Coach Navigation
8. Route Definitions
9. GoRouter Structure
10. Route Guards
11. Deep Linking
12. Push Notification Navigation
13. AI Navigation
14. Error Navigation
15. Session Management
16. API Navigation Mapping

---

# 1. Navigation Principles

Navigation must be:

- Fast
- Predictable
- Role-aware
- Deep-link enabled
- Offline tolerant
- Gesture friendly
- State preserving

Maximum navigation depth:

```
3 Levels
```

Avoid long navigation chains.

---

# 2. Navigation Layers

```text
Application

│

├── Authentication

├── Main Navigation

│      ├── Bottom Navigation

│      ├── Nested Navigation

│      ├── Modal Navigation

│      └── Full Screen Navigation

│

└── Deep Link Navigation
```

---

# 3. Application Startup Flow

```text
Launch App

↓

Splash Screen

↓

Check App Version

↓

Initialize Services

↓

Read Secure Storage

↓

JWT Available?

├── No

│ ↓

│ Login

│

└── Yes

↓

Validate Token

↓

Load Profile

↓

Determine Role

↓

Dashboard
```

---

# 4. Authentication Flow

```text
Welcome

↓

Login

↓

OTP

↓

Biometric Registration

↓

Dashboard
```

Forgot Password

```
Login

↓

Forgot Password

↓

OTP

↓

New Password

↓

Success
```

---

# 5. Athlete Navigation

Bottom Navigation

```text
🏠 Home

🏋 Training

🤖 AI Coach

🔔 Notifications

👤 Profile
```

---

## Home Flow

```text
Dashboard

↓

Attendance

↓

Training Details

↓

Exercise Details
```

---

## Performance Flow

```text
Dashboard

↓

Performance

↓

Skill Report

↓

Coach Feedback
```

---

## Tournament Flow

```text
Dashboard

↓

Tournament

↓

Tournament Details

↓

Register

↓

Payment

↓

Confirmation
```

---

## Payment Flow

```text
Dashboard

↓

Invoices

↓

Invoice Details

↓

Payment Gateway

↓

Success

↓

Receipt
```

---

## AI Coach Flow

```text
Dashboard

↓

AI Coach

↓

Conversation

↓

Knowledge Search

↓

Suggested Exercises

↓

Training Plan
```

---

# 6. Parent Navigation

Bottom Navigation

```text
Home

Children

Notifications

Payments

Profile
```

Children

↓

Child Dashboard

↓

Attendance

↓

Performance

↓

Coach Feedback

↓

Payments

---

# 7. Coach Navigation

Bottom Navigation

```text
Home

Athletes

Calendar

Notifications

Profile
```

Athletes

↓

Athlete Details

↓

Attendance

↓

Performance

↓

Training

↓

Evaluation

---

# 8. Route Definitions

```
/

splash

welcome

login

otp

dashboard

attendance

training

training/:id

performance

performance/:id

payments

payments/:id

wallet

tournaments

tournaments/:id

events

events/:id

notifications

profile

settings

ai

chat

documents

help

feedback
```

---

# 9. GoRouter Structure

```dart
GoRouter(

routes: [

SplashRoute(),

LoginRoute(),

DashboardRoute(),

TrainingRoute(),

PerformanceRoute(),

TournamentRoute(),

PaymentRoute(),

NotificationRoute(),

AIRoute(),

ProfileRoute()

]

)
```

Every feature owns its routes.

---

# 10. Navigation Guards

## Public Routes

Welcome

Login

OTP

Privacy Policy

Terms

---

## Protected Routes

Dashboard

Training

Attendance

Performance

Payments

Notifications

AI Coach

Profile

---

## Admin Routes

Not available in Athlete App.

---

# Route Guard Logic

```text
Authenticated?

↓

No

↓

Login

↓

Yes

↓

Role Valid?

↓

No

↓

Access Denied

↓

Yes

↓

Navigate
```

---

# 11. Deep Linking

Supported

```
sportsgurukul://training/567

sportsgurukul://attendance

sportsgurukul://payments/100

sportsgurukul://tournament/22

sportsgurukul://ai/chat

sportsgurukul://notification/554

sportsgurukul://coach/feedback
```

---

Deep Link Validation

Validate

JWT

Permissions

Role

Academy

Subscription

Feature Flag

---

# 12. Push Notification Navigation

Notification

↓

Open App

↓

Authenticate

↓

Resolve Target

↓

Navigate

Examples

Training Reminder

↓

Training Details

Payment Reminder

↓

Invoice Details

Tournament Reminder

↓

Tournament

Coach Message

↓

Chat

AI Recommendation

↓

AI Coach

---

# 13. AI Navigation

```text
AI Coach

↓

Conversation

↓

Suggested Actions

↓

Knowledge Search

↓

Training Recommendation

↓

Performance Insights
```

History

↓

Conversation Details

↓

Regenerate

↓

Share

---

# 14. Error Navigation

Network Error

↓

Retry

Unauthorized

↓

Login

Forbidden

↓

Access Denied

404

↓

Dashboard

Maintenance

↓

Maintenance Screen

---

# 15. Session Timeout

App Idle

↓

Refresh Token

↓

Expired?

↓

Login

↓

Restore Previous Route

```

Never lose user navigation state.

---

# 16. Offline Navigation

Available Offline

Dashboard Cache

Training

Attendance History

Notifications

AI History

Documents

Unavailable

Payments

Tournament Registration

Live Scores

Video Streaming

---

# 17. API Navigation Mapping

| Screen | API |
|---------|-----|
| Dashboard | GET /api/v1/dashboard |
| Training | GET /api/v1/training |
| Attendance | GET /api/v1/attendance |
| Performance | GET /api/v1/performance |
| Payments | GET /api/v1/finance/invoices |
| Notifications | GET /api/v1/notifications |
| AI Coach | POST /api/v1/ai/chat |
| Profile | GET /api/v1/profile |

---

# 18. Navigation Analytics

Track

Screen Open

Screen Duration

Button Click

Back Navigation

Deep Link Source

Push Navigation

AI Entry

Payment Funnel

Training Completion

Tournament Registration

---

# 19. Navigation Animations

Page Push

250ms

Modal

300ms

Bottom Sheet

300ms

Hero

350ms

Back Navigation

Native

---

# 20. Acceptance Criteria

- GoRouter only
- Deep-link ready
- Role-based routing
- Authentication guards
- Offline aware
- State preserved
- Navigation analytics
- Backend API aligned
- Feature-based route ownership
- Testable navigation

---

# Related Documents

- 00-Mobile-App-Vision.md
- 01-Design-System.md
- 02-Information-Architecture.md
- 04-API-Integration-Guide.md
- 05-State-Management.md

---

**End of Document**
```
