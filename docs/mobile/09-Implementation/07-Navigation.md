---
title: Navigation Architecture
module: Implementation
platform: Flutter
navigation: go_router
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Navigation Architecture

> Defines the official navigation architecture for Sports Gurukul using `go_router`. This document standardizes routing, authentication guards, role-based navigation, deep linking, nested navigation, and state restoration across all mobile applications.

---

# Table of Contents

1. Overview
2. Objectives
3. Navigation Principles
4. Route Hierarchy
5. Application Flow
6. Shell Navigation
7. Authentication Guards
8. Authorization Guards
9. Route Structure
10. Deep Link Integration
11. Bottom Navigation
12. Nested Navigation
13. Navigation State
14. Route Transitions
15. Error Routes
16. Analytics
17. Testing
18. Acceptance Criteria

---

# 1. Overview

Navigation must support

✓ Feature-first architecture

✓ Authentication

✓ Authorization

✓ Deep Linking

✓ Multiple User Roles

✓ State Restoration

✓ Offline Support

---

# 2. Objectives

Provide

- Predictable navigation
- Fast transitions
- Secure routing
- Modular route registration
- Easy testing
- Deep link compatibility

---

# 3. Navigation Principles

✓ Route-based navigation

✓ Declarative routing

✓ Feature-owned routes

✓ Typed route parameters

✓ Centralized guards

✓ No hardcoded route strings

---

# 4. Route Hierarchy

```text
Splash

↓

Authentication

↓

Role Selection (if applicable)

↓

Main Shell

├── Dashboard
├── Training
├── Attendance
├── Performance
├── Tournaments
├── Messages
├── Profile
└── Settings
```

---

# 5. Application Flow

```text
App Launch

↓

Splash

↓

Authentication Check

↓

Token Valid?

├── No → Login

└── Yes

↓

Role Validation

↓

Dashboard
```

---

# 6. Shell Navigation

Main application uses a ShellRoute.

```text
Shell

├── Dashboard

├── Training

├── Attendance

├── Messages

├── Profile
```

Bottom navigation remains persistent.

---

# 7. Authentication Guard

Protected Routes

Dashboard

Attendance

Payments

Medical

Documents

Messages

Workflow

```text
Navigate

↓

Authenticated?

├── Yes

↓

Continue

└── No

↓

Login

↓

Return to Original Route
```

---

# 8. Authorization Guard

Role Examples

Athlete

Coach

Parent

Academy Admin

Super Admin

Example

```text
Medical Records

↓

Coach?

↓

Allowed

↓

Athlete?

↓

Own Records Only

↓

Others

↓

403 Screen
```

---

# 9. Route Structure

Public Routes

```
/

/login

/register

/forgot-password

/privacy

/terms
```

Protected Routes

```
/dashboard

/training

/attendance

/performance

/tournaments

/messages

/profile

/settings

/help
```

Parameterized Routes

```
/training/:id

/tournament/:id

/document/:id

/message/:id
```

---

# 10. Feature Route Registration

Each feature registers

Routes

Deep Links

Permissions

Analytics Events

Example

```text
training/

presentation/

routes/

training_routes.dart
```

---

# 11. Deep Link Integration

Examples

```
sportsgurukul://training/TR123

sportsgurukul://attendance

sportsgurukul://tournament/T001

sportsgurukul://profile
```

Flow

```text
Deep Link

↓

Authentication

↓

Permission Check

↓

Navigate
```

---

# 12. Bottom Navigation

Tabs

Dashboard

Training

Attendance

Messages

Profile

Rules

Preserve state

Independent navigation stacks

Badge support

---

# 13. Nested Navigation

Example

```text
Training

↓

Training Details

↓

Session Details

↓

Exercise Details
```

Back navigation remains within the feature.

---

# 14. Navigation State Restoration

Restore

Selected Tab

Scroll Position

Filters

Search Query

Expanded Panels

Unfinished Forms

---

# 15. Route Parameters

Examples

```
trainingId

attendanceId

eventId

paymentId

documentId
```

Use typed parsing instead of raw string access.

---

# 16. Route Transitions

Default

Material Transition

Custom

Fade

Slide

Scale

Bottom Sheet

Hero

Platform-specific transitions where appropriate.

---

# 17. Error Routes

404

Route Not Found

403

Permission Denied

500

Unexpected Navigation Error

Offline

Offline Route

---

# 18. Navigation Analytics

Track

```
screen_opened

screen_closed

deep_link_opened

tab_changed

navigation_failed

permission_denied

back_navigation

```

---

# 19. State Management

Navigation State

↓

Riverpod

↓

GoRouter

↓

UI

Navigation decisions should be driven by application state, not widget-local logic.

---

# 20. Testing

Verify

Authentication Redirect

Role Authorization

Deep Links

Back Navigation

Bottom Navigation

Nested Navigation

State Restoration

404 Handling

403 Handling

---

# 21. Folder Structure

```text
app/

router/

app_router.dart

route_names.dart

route_paths.dart

route_guards.dart

navigation_service.dart

shell_routes.dart

transitions.dart
```

Feature Routes

```text
features/

training/

presentation/

routes/

training_routes.dart
```

---

# 22. Best Practices

✓ Feature-owned routes

✓ Centralized guards

✓ Typed route parameters

✓ State restoration

✓ Analytics integration

✓ Deep link support

✓ Independent navigation stacks

---

# 23. Anti-Patterns

❌ Hardcoded route strings

❌ Navigation from repositories

❌ Business logic inside route builders

❌ Global navigation hacks

❌ Duplicate route definitions

❌ Manual authentication checks in every screen

---

# 24. Performance Goals

Initial Route

<300 ms

Screen Transition

<250 ms

Deep Link Resolution

<500 ms

State Restoration

<200 ms

---

# 25. Acceptance Criteria

✓ go_router implemented

✓ Shell navigation configured

✓ Authentication guards

✓ Authorization guards

✓ Deep links supported

✓ Nested navigation

✓ State restoration

✓ Analytics integrated

✓ Testable architecture

✓ Enterprise ready

---

# Related Documents

08-State-Management.md

Riverpod Architecture

Deep Linking

Flutter Project Architecture

Security & Compliance

---

# Future Enhancements

- Dynamic feature routes
- Remote-configurable navigation
- Feature flag-based routing
- Multi-window support
- Desktop navigation adaptations
- Navigation debugging tools

---

# End of Document
