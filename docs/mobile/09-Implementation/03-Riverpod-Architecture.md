---
title: Riverpod Enterprise Architecture
module: Implementation
platform: Flutter
architecture: Riverpod 3.x
version: 1.0
status: Approved
owner: Mobile Architecture Team
---

# Riverpod Enterprise Architecture

> Defines the official Riverpod implementation guidelines for Sports Gurukul. All mobile applications must use these patterns to ensure consistency, maintainability, performance, and testability.

---

# Table of Contents

1. Overview
2. Objectives
3. Architecture
4. Provider Hierarchy
5. Provider Types
6. Dependency Injection
7. Feature Structure
8. State Lifecycle
9. Async State
10. Error Handling
11. Offline Integration
12. Performance
13. Testing
14. Anti-Patterns
15. Acceptance Criteria

---

# 1. Overview

Riverpod is used for

✓ Dependency Injection

✓ State Management

✓ Async Data Loading

✓ Feature Communication

✓ Caching

✓ Offline Synchronization

✓ Configuration

✓ Authentication

---

# 2. Objectives

Provide

- Predictable state
- Testable architecture
- Minimal widget rebuilds
- Dependency injection
- Offline support
- Modular features
- Easy debugging

---

# 3. Architecture

```text
UI

↓

ConsumerWidget

↓

Riverpod Provider

↓

Use Case

↓

Repository

↓

Datasource

↓

API / SQLite
```

Presentation never talks directly to repositories.

---

# 4. Provider Hierarchy

Global Providers

```
ThemeProvider

LocaleProvider

AuthenticationProvider

ConnectivityProvider

AnalyticsProvider

SyncProvider

NavigationProvider
```

Feature Providers

```
TrainingProvider

AttendanceProvider

TournamentProvider

PerformanceProvider

PaymentProvider

MedicalProvider

DocumentProvider

AIProvider
```

---

# 5. Recommended Provider Types

## Provider

Immutable objects

Configuration

Utilities

Services

Example

```
ApiClientProvider

LoggerProvider
```

---

## StateProvider

Only for simple UI state

Example

```
Selected Tab

Search Text

Bottom Navigation Index
```

Avoid business logic.

---

## NotifierProvider

Business state

Example

```
Attendance

Training

Settings

Profile
```

---

## AsyncNotifierProvider

API-driven features

Examples

```
Training Schedule

Leaderboard

Wallet

Documents

Medical Dashboard

Achievements
```

---

## FutureProvider

Read-only operations

Examples

```
App Version

Configuration

Build Info
```

---

## StreamProvider

Real-time data

Examples

```
Chat Messages

Live Scores

Notifications

WebSocket Status
```

---

# 6. Dependency Injection

Use constructor injection.

```text
Provider

↓

Repository

↓

Datasource

↓

API Client
```

Example

```dart
final trainingRepositoryProvider =
    Provider<TrainingRepository>((ref) {
  return TrainingRepositoryImpl(
    api: ref.read(apiProvider),
    database: ref.read(databaseProvider),
  );
});
```

Never instantiate dependencies inside providers.

---

# 7. Feature Structure

```text
training/

presentation/

providers/

training_provider.dart

training_state.dart

training_notifier.dart
```

Keep providers inside their feature.

---

# 8. State Lifecycle

```text
Loading

↓

Success

↓

Refresh

↓

Updated
```

Error

↓

Retry

↓

Recovered

---

# 9. Async State Pattern

```dart
AsyncLoading

AsyncData

AsyncError
```

UI must handle all three states.

---

# 10. State Model

Example

```dart
TrainingState

- sessions
- selectedSession
- filters
- loading
- error
```

Avoid large state objects.

---

# 11. AutoDispose Strategy

Use

```
AutoDispose

for

Search

Temporary Screens

Filters

OTP

Login
```

Avoid

AutoDispose

for

Authentication

Theme

Navigation

Settings

---

# 12. Family Providers

Use when parameters change.

Example

```dart
trainingProvider(trainingId)

attendanceProvider(date)

documentProvider(documentId)
```

---

# 13. Repository Integration

Provider

↓

Use Case

↓

Repository Interface

↓

Repository Implementation

↓

Datasource

↓

API

---

# 14. Offline Integration

Read Flow

```text
SQLite

↓

Provider

↓

UI
```

Write Flow

```text
Provider

↓

SQLite

↓

Sync Queue

↓

Background Upload
```

Providers should remain unaware of synchronization internals.

---

# 15. Refresh Strategy

Manual Refresh

↓

Pull To Refresh

↓

Background Refresh

↓

Periodic Refresh

↓

Network Restored

---

# 16. Error Handling

Return

```dart
AsyncError
```

Never

```
throw Exception()
```

directly to the UI.

Failure Types

Network

Validation

Authentication

Permission

Business

Unknown

---

# 17. Performance Rules

Prefer

```
ref.watch(select())
```

to observe only required fields.

Split large providers into smaller feature-specific providers.

Avoid rebuilding entire screens.

---

# 18. Provider Communication

Allowed

```text
AttendanceProvider

↓

TrainingRepository
```

Not Allowed

```text
AttendanceProvider

↓

TrainingProvider
```

Communication should occur through repositories or application services.

---

# 19. Caching

Provider

↓

Repository

↓

Memory Cache

↓

SQLite

↓

API

Always return cached data first when available.

---

# 20. Authentication Flow

```text
Login

↓

AuthProvider

↓

Token

↓

Secure Storage

↓

Authenticated State

↓

Refresh Automatically
```

---

# 21. Testing

Test

- Providers
- Notifiers
- State transitions
- Error handling
- Refresh
- Retry
- Offline mode

Mock

Repositories

API Client

Database

---

# 22. Logging

Log

Provider Created

Disposed

Refresh

Failure

Retry

Duration

Correlation ID

Do not log sensitive information.

---

# 23. Flutter Folder Structure

```text
presentation/

providers/

training_provider.dart

training_notifier.dart

training_state.dart

training_selectors.dart
```

---

# 24. Anti-Patterns

❌ Business logic inside widgets

❌ API calls inside widgets

❌ Global mutable state

❌ One provider for the whole application

❌ Circular provider dependencies

❌ Large state models

❌ Calling ref.watch() unnecessarily

❌ Manual dependency creation

---

# 25. Performance Targets

Provider Initialization

<10 ms

State Update

<5 ms

Refresh

<300 ms

Provider Rebuild

Minimal

---

# 26. Acceptance Criteria

✓ Feature-scoped providers

✓ Dependency injection implemented

✓ Async state handled

✓ Offline integration supported

✓ Minimal rebuilds

✓ Providers fully testable

✓ Error handling standardized

✓ AutoDispose strategy documented

✓ Performance optimized

✓ Enterprise ready

---

# Related Documents

04-Dio-API-Architecture.md

05-Repository-Pattern.md

06-Local-Database.md

07-Navigation.md

08-State-Management-Standards.md

09-Coding-Standards.md

---

# Future Enhancements

- Riverpod code generation
- Provider performance dashboard
- Automatic provider dependency visualization
- Offline-aware providers
- Feature-level state persistence
- Time-travel debugging integration

---

# End of Document
