---
title: Sports Gurukul Mobile Information Architecture
version: 1.0
status: Draft
owner: Product Team
---

# 📱 Sports Gurukul Mobile Information Architecture

> Defines the complete application hierarchy, navigation structure, feature modules, backend integration boundaries, and user flows for all mobile applications.

---

# Table of Contents

1. Purpose
2. Application Ecosystem
3. User Roles
4. App Architecture
5. Feature Hierarchy
6. Athlete App
7. Parent App
8. Coach App
9. Shared Modules
10. Deep Linking
11. Route Architecture
12. Feature Ownership
13. Backend API Mapping
14. Navigation Rules
15. Security Rules
16. Future Modules

---

# 1. Purpose

This document defines

- Complete application hierarchy
- Navigation architecture
- Feature ownership
- Backend API ownership
- Route structure
- Mobile module boundaries

This document must remain synchronized with

- Backend APIs
- CQRS Modules
- Database
- Authorization Policies

---

# 2. Mobile Application Ecosystem

```text
                    Sports Gurukul

                    Mobile Platform
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
        ▼                  ▼                  ▼
 Athlete App         Parent App         Coach App
        │                  │                  │
        └──────────────────┼──────────────────┘
                           │
                     Shared Platform
                           │
        Authentication
        Notifications
        AI Platform
        Payments
        Profile
        Chat
        Settings
```

---

# 3. User Roles

## Athlete

Primary User

Permissions

- Own Profile
- Own Attendance
- Own Training
- Own Performance
- Own Payments
- Own Notifications
- AI Coach

---

## Parent

Permissions

- Child Profile
- Attendance
- Fees
- Performance
- Coach Feedback
- Notifications

---

## Coach

Permissions

- Assigned Athletes
- Attendance
- Training
- Performance
- Communication
- AI Assistant

---

# 4. Mobile Architecture

```text
Flutter Application

Presentation

↓

Feature Module

↓

Application

↓

Repository

↓

REST Client

↓

Backend API

↓

CQRS

↓

Domain
```

---

# 5. Feature Modules

```text
features/

authentication/

dashboard/

profile/

training/

attendance/

performance/

tournament/

events/

payments/

wallet/

notifications/

chat/

ai/

settings/

shared/
```

Each module is independently deployable.

---

# 6. Athlete Application

## Root Navigation

```text
Athlete App

Home

Training

AI Coach

Notifications

Profile
```

---

## Home

Children

Dashboard

Upcoming Training

Attendance

Today's Goal

Quick Actions

Latest Announcement

AI Insight

---

## Dashboard

Contains

Greeting

Performance Summary

Attendance %

Fitness Score

Upcoming Sessions

Tournament

Notifications

Coach Message

Quick Payment

AI Recommendation

---

## Profile

Contains

Personal Information

Sports Information

Medical

Emergency Contacts

Documents

Achievements

Certificates

Settings

---

## Training

Contains

Today's Training

Calendar

Exercise Library

Coach Notes

Workout History

Attendance

Videos

Downloads

---

## Attendance

Contains

Monthly Attendance

Daily Attendance

Check-In

Leave Request

Attendance Analytics

---

## Performance

Contains

Statistics

Fitness

Speed

Strength

Agility

Coach Rating

Progress

Goals

Reports

---

## Tournament

Contains

Upcoming

Registration

Fixtures

Live Score

Results

Rankings

Certificates

Gallery

---

## Events

Contains

Upcoming Events

Past Events

Calendar

Registration

Gallery

---

## Payments

Contains

Invoices

Receipts

Pending Fees

Online Payment

Scholarship

Discount

---

## Wallet

Contains

Credits

Refunds

Rewards

Transactions

---

## AI Coach

Contains

Chat

Training Advice

Nutrition

Goal Planning

Performance Analysis

Knowledge Search

Voice Chat

History

---

## Notifications

Contains

Announcements

Training

Payments

Events

Tournament

System

AI

---

## Settings

Contains

Theme

Language

Notifications

Privacy

Security

Biometric

Logout

---

# 7. Parent Application

```text
Dashboard

↓

Children

↓

Attendance

↓

Performance

↓

Payments

↓

Communication

↓

Profile
```

---

Parent can switch between multiple children.

---

# 8. Coach Application

```text
Dashboard

↓

Athletes

↓

Attendance

↓

Training

↓

Performance

↓

Calendar

↓

Communication

↓

Profile
```

---

Coach can manage

Training Plans

Attendance

Performance

AI Recommendations

---

# 9. Shared Modules

Authentication

Profile

Notifications

Settings

AI

Payments

Documents

Help

Feedback

---

# 10. Route Structure

```text
/

login

otp

dashboard

training

attendance

performance

tournament

events

payments

wallet

notifications

chat

profile

settings
```

---

Deep Link Examples

```
sportsgurukul://training/123

sportsgurukul://payment/445

sportsgurukul://tournament/99

sportsgurukul://ai/chat

sportsgurukul://profile
```

---

# 11. Backend Module Mapping

| Mobile Module  | Backend Module    |
| -------------- | ----------------- |
| Authentication | Identity          |
| Dashboard      | Dashboard API     |
| Profile        | User Management   |
| Training       | Training Platform |
| Attendance     | Attendance        |
| Tournament     | Tournament        |
| Events         | Event Platform    |
| Payments       | Finance           |
| Wallet         | Finance           |
| AI Coach       | AI Platform       |
| Notifications  | Communication     |
| Chat           | Communication     |
| Settings       | Identity          |

---

# 12. API Ownership

Authentication

Identity Service

Training

Training Service

Payments

Finance Service

Notifications

Communication Service

AI

AI Platform

Tournament

Tournament Service

Events

Event Service

---

# 13. Navigation Rules

Maximum navigation depth

3 Levels

Back button always available

No hidden navigation

Bottom Navigation fixed

Top AppBar contextual

FAB only where required

---

# 14. Authorization

Every feature validates

JWT

Role

Permissions

Academy

Subscription

Feature Flags

---

# 15. Offline Support

Available Offline

Profile

Training Schedule

Attendance History

Performance Summary

Notifications

AI History

Pending Payments

Documents

Sync when online

---

# 16. Module Dependency

```text
Dashboard

↓

Training

↓

Attendance

↓

Performance

↓

Tournament

↓

Payments

↓

Notifications

↓

AI Coach
```

No feature should directly access another feature's state.

Communication occurs through repositories and shared services.

---

# 17. Feature Package Structure

```text
features/

training/

presentation/

application/

domain/

data/

widgets/

models/

providers/

repository/

api/
```

Every feature follows Clean Architecture.

---

# 18. Acceptance Criteria

✅ Feature based architecture

✅ Clean Architecture

✅ Independent modules

✅ Backend aligned

✅ Deep Linking

✅ Offline ready

✅ AI ready

✅ Scalable

✅ Testable

---

# Related Documents

- 00-Mobile-App-Vision.md
- 01-Design-System.md
- 03-Navigation-Architecture.md
- 04-API-Integration-Guide.md
- 05-State-Management.md

---

**End of Document**
