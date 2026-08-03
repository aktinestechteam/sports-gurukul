---
title: State Management Standards
module: Implementation
platform: Flutter
architecture: Riverpod
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# State Management Standards

> Defines the official state management standards for Sports Gurukul mobile applications. This document establishes how UI state, business state, application state, offline state, and session state should be managed consistently across all features.

---

# Table of Contents

1. Overview
2. Objectives
3. State Classification
4. State Ownership
5. Provider Selection Guide
6. Immutable State
7. State Lifecycle
8. UI State Standards
9. Feature State Standards
10. Session State
11. Application State
12. Offline State
13. Optimistic Updates
14. Error Handling
15. Loading States
16. State Persistence
17. Memory Management
18. Performance
19. Debugging
20. Acceptance Criteria

---

# 1. Overview

Every piece of state must have

✓ Single Owner

✓ Predictable Lifecycle

✓ Immutable Updates

✓ Testability

✓ Clear Responsibility

Never duplicate the same state in multiple providers.

---

# 2. Objectives

Provide

- Predictable state flow
- Offline support
- High performance
- Easy debugging
- Minimal rebuilds
- Easy testing

---

# 3. State Classification

## UI State

Temporary

Examples

- Selected Tab
- Search Text
- Dialog Open
- Bottom Sheet
- Selected Filter

---

## Feature State

Business specific

Examples

- Attendance
- Training
- Payments
- Documents
- AI Chat

---

## Session State

Valid only while logged in

Examples

- User
- JWT
- Permissions
- Academy

---

## Application State

Global

Examples

- Theme
- Locale
- Connectivity
- Feature Flags

---

## Offline State

Examples

- Sync Queue
- Pending Uploads
- Cached Records
- Last Sync Time

---

# 4. State Ownership

| State          | Owner              |
| -------------- | ------------------ |
| Theme          | ThemeProvider      |
| Locale         | LocaleProvider     |
| Authentication | AuthProvider       |
| Training       | TrainingProvider   |
| Attendance     | AttendanceProvider |
| Payments       | PaymentProvider    |
| Medical        | MedicalProvider    |
| Documents      | DocumentProvider   |
| Chat           | ChatProvider       |
| Sync Queue     | SyncProvider       |

Each state has exactly one owner.

---

# 5. Provider Selection Guide

| Requirement        | Provider              |
| ------------------ | --------------------- |
| Configuration      | Provider              |
| Simple UI Value    | StateProvider         |
| Business Logic     | NotifierProvider      |
| API Data           | AsyncNotifierProvider |
| Real-time Stream   | StreamProvider        |
| Parameterized Data | Family Provider       |

---

# 6. Immutable State

All state objects should be immutable.

Recommended

```
freezed
```

Example

```
TrainingState

copyWith()

==

hashCode
```

Never mutate collections directly.

---

# 7. State Lifecycle

```text
Created

↓

Loading

↓

Loaded

↓

Updated

↓

Refreshing

↓

Disposed
```

Error

↓

Retry

↓

Recovered

---

# 8. UI State Standards

Keep UI state local to the feature.

Examples

Search

Selected Tab

Expanded Panel

Dialog Visibility

Avoid placing temporary UI state into global providers.

---

# 9. Feature State

Each feature owns

State

Provider

Notifier

Selectors

Example

```
training/

training_state.dart

training_provider.dart

training_notifier.dart

training_selectors.dart
```

---

# 10. Session State

Contains

Current User

Permissions

Academy

JWT Status

Current Role

Session Timeout

Destroyed on logout.

---

# 11. Application State

Examples

Theme

Language

Connectivity

Device

Feature Flags

Application state survives screen changes.

---

# 12. Offline State

Tracks

Pending Sync

Pending Upload

Pending Download

Conflict Count

Last Sync

Current Network

Offline Banner

---

# 13. Optimistic Updates

Workflow

```text
User Action

↓

Update UI Immediately

↓

Save Locally

↓

Queue Sync

↓

Server

↓

Success

or

Rollback
```

Suitable for

Attendance

Training Progress

Settings

Favorites

Not suitable for

Payment Confirmation

Critical Medical Records

Identity Verification

---

# 14. Loading State

Every screen supports

Initial Loading

Refresh

Incremental Loading

Pagination

Background Refresh

Skeleton UI preferred over blocking spinners.

---

# 15. Error State

Display

Friendly Message

Retry Action

Technical details only in logs.

Examples

Network Error

Validation Error

Permission Denied

Offline

Server Error

---

# 16. Empty State

Every list must support

No Data

No Search Results

No Internet

No Permissions

No Upcoming Events

Each empty state should include guidance or a primary action where appropriate.

---

# 17. State Persistence

Persist

Theme

Language

Authentication

Filters (optional)

Recent Searches

Downloads

Do Not Persist

OTP

Passwords

Temporary Dialog State

Loading Indicators

---

# 18. Memory Management

Dispose

Temporary Providers

Controllers

Streams

Timers

AutoDispose where appropriate.

---

# 19. Performance Rules

Use

```
ref.watch(select())
```

Split large providers.

Avoid

Watching entire objects

Deep widget rebuilds

Unnecessary listeners

---

# 20. State Synchronization

Background changes

↓

Repository

↓

Provider Update

↓

UI Refresh

Never update UI directly from background services.

---

# 21. State Debugging

Log

State Created

State Updated

Refresh

Retry

Dispose

Error

Duration

Correlation ID

Do not log sensitive user data.

---

# 22. Folder Structure

```
presentation/

providers/

training_provider.dart

training_notifier.dart

training_state.dart

training_selectors.dart
```

---

# 23. Naming Standards

Examples

```
TrainingState

TrainingNotifier

TrainingProvider

TrainingSelector

TrainingEvent
```

---

# 24. Anti-Patterns

❌ Mutable state

❌ Business logic inside widgets

❌ API calls from UI

❌ Duplicate providers

❌ One global app state

❌ Nested provider dependencies

❌ Manual refresh everywhere

❌ UI updating database directly

---

# 25. Performance Targets

State Update

<5 ms

Provider Initialization

<10 ms

Refresh

<300 ms

Memory Growth

Stable over long sessions

---

# 26. Acceptance Criteria

✓ Single owner for every state

✓ Immutable state models

✓ Optimistic updates documented

✓ Offline-aware state

✓ Loading/error/empty states standardized

✓ Memory lifecycle defined

✓ Provider usage standardized

✓ Performance optimized

✓ Fully testable

✓ Enterprise ready

---

# Related Documents

Riverpod Architecture

Repository Pattern

Local Database

Navigation

Offline Synchronization

Flutter Project Architecture

---

# Future Enhancements

- Time-travel state debugging
- State replay for bug reproduction
- Provider dependency visualization
- Automatic state persistence policies
- DevTools integration for state inspection

---

# End of Document
