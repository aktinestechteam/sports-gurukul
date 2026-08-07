---
title: Sports Gurukul Mobile State Management
version: 1.0
status: Draft
owner: Mobile Architecture Team
platform: Flutter
state_management: Riverpod
architecture: Clean Architecture
---

# ⚡ State Management Architecture

> Defines how application state, business logic, API communication, caching, offline synchronization, and dependency injection are managed throughout the Sports Gurukul Flutter application.

---

# Table of Contents

1. Purpose
2. Architecture Principles
3. Feature Architecture
4. Riverpod Architecture
5. Provider Hierarchy
6. Repository Pattern
7. Use Cases
8. DTO Mapping
9. Async State
10. Authentication State
11. Navigation State
12. AI State
13. Notification State
14. Offline Synchronization
15. Dependency Injection
16. Folder Structure
17. Coding Standards
18. Acceptance Criteria

---

# 1. Purpose

Every feature must follow a consistent architecture.

UI should never contain business logic.

Business logic should never know about Flutter widgets.

Repositories should never know about UI.

---

# 2. Architecture Overview

```text
Presentation

↓

Riverpod Provider

↓

Application Service

↓

Use Case

↓

Repository

↓

REST API

↓

.NET Backend

↓

CQRS

↓

Database
```

---

# 3. Clean Architecture

Each feature is isolated.

```
features/

attendance/

presentation/

application/

domain/

data/

shared/
```

---

# Presentation Layer

Contains

- Screens
- Widgets
- Dialogs
- Bottom Sheets
- Providers
- Controllers

No API calls allowed.

---

# Application Layer

Contains

- Use Cases
- Business Rules
- Validation
- Services

---

# Domain Layer

Contains

- Entities
- Repository Interfaces
- Value Objects

---

# Data Layer

Contains

- DTOs
- API Clients
- Repository Implementations
- Mappers

---

# 4. Riverpod Provider Hierarchy

```text
ProviderScope

↓

AppProvider

↓

AuthenticationProvider

↓

Feature Providers

↓

Widget Providers
```

---

# Provider Types

Provider

StateProvider

FutureProvider

StreamProvider

NotifierProvider

AsyncNotifierProvider

FamilyProvider

---

# 5. Repository Pattern

Every feature owns its repository.

Example

```dart
AttendanceRepository

TrainingRepository

PaymentRepository

NotificationRepository

AIRepository
```

Repositories expose domain models only.

---

# Repository Flow

```text
UI

↓

Provider

↓

Repository

↓

API Client

↓

Backend
```

---

# 6. Use Cases

Every business action is implemented as a use case.

Examples

```
LoginUseCase

LoadDashboardUseCase

CheckInAttendanceUseCase

PayInvoiceUseCase

StartAIConversationUseCase

UpdateProfileUseCase
```

---

# 7. DTO Mapping

Never expose API DTOs to UI.

```text
JSON

↓

Response DTO

↓

Mapper

↓

Domain Entity

↓

View Model

↓

Widget
```

---

# Example

```
TrainingResponseDto

↓

TrainingMapper

↓

Training

↓

TrainingViewModel
```

---

# 8. Async State Management

Every async operation has four states.

```text
Loading

↓

Success

↓

Error

↓

Retry
```

Widgets react to state changes only.

---

# 9. Authentication State

Managed globally.

Contains

```
JWT

Refresh Token

Current User

Current Academy

Permissions

Role

Subscription
```

---

# Login Flow

```text
Login

↓

Authentication Provider

↓

Repository

↓

API

↓

Secure Storage

↓

Dashboard
```

---

# 10. Dashboard State

Dashboard state combines

Attendance

Training

Notifications

Performance

Payments

Upcoming Events

AI Suggestions

Each widget loads independently.

Dashboard failure should not fail the entire page.

---

# 11. Navigation State

Navigation state stores

Current Route

Tab

Deep Link

Back Stack

Selected Athlete

Current Academy

---

# 12. Notification State

Real-time updates

Unread Count

Push Messages

Announcement Feed

Notification History

Background Refresh

---

# 13. AI State

AI Conversation

Streaming Messages

Typing Indicator

Prompt History

Citation Sources

Token Usage

Suggested Prompts

Conversation Memory

---

Streaming State

```text
Idle

↓

Connecting

↓

Streaming

↓

Completed

↓

Error
```

---

# 14. Attendance State

Contains

Today's Attendance

History

Leave Requests

Analytics

Sync Queue

---

# 15. Training State

Contains

Today's Training

Calendar

Exercises

Coach Notes

Videos

Downloads

---

# 16. Payment State

Contains

Invoices

Receipts

Wallet

Pending Payments

Payment Status

Scholarships

---

# 17. Offline State

Stores

Attendance Queue

Feedback Queue

Profile Updates

Downloaded Training

AI History

Notifications

Documents

---

Sync Flow

```text
Offline Action

↓

Local Database

↓

Sync Queue

↓

Network Available

↓

Background Sync

↓

Server

↓

Success
```

---

# 18. Dependency Injection

Use Riverpod only.

Never use GetIt.

Example

```
ApiClientProvider

↓

TrainingRepositoryProvider

↓

TrainingUseCaseProvider

↓

TrainingControllerProvider
```

---

# 19. Error State

Every feature must expose

Loading

Success

Error

Retry

Offline

Unauthorized

---

# 20. Caching

Cache

Dashboard

Training

Attendance

Profile

Notifications

AI History

Never Cache

Passwords

Tokens

Payment Processing

---

# 21. Folder Structure

```
features/

attendance/

presentation/

pages/

widgets/

controllers/

providers/

application/

usecases/

services/

domain/

entities/

repositories/

data/

dto/

mapper/

repository/

datasource/
```

---

# 22. Feature Example

Attendance

```
AttendancePage

↓

AttendanceController

↓

AttendanceProvider

↓

AttendanceUseCase

↓

AttendanceRepository

↓

AttendanceApi

↓

Backend
```

---

# 23. Global Providers

```
AuthenticationProvider

ThemeProvider

LanguageProvider

ConnectivityProvider

NotificationProvider

AIProvider

ProfileProvider

NavigationProvider
```

---

# 24. Optimistic Updates

Used for

Attendance

Profile

Feedback

Bookmarks

Favorites

UI updates immediately.

Rollback if API fails.

---

# 25. Streaming Support

Supports

AI Chat

Live Match Scores

Coach Messages

Notifications

Workout Progress

---

# 26. Performance Rules

Never rebuild entire screen.

Use ConsumerWidget selectively.

Split widgets into small reusable components.

Keep providers feature-specific.

---

# 27. Coding Standards

Never call API from widgets.

Never store mutable global state.

Never expose DTOs.

Always use immutable models.

Always map API responses.

Always use repository interfaces.

Always handle loading, success, and error states.

---

# 28. Testing Strategy

Test

Repository

Use Cases

Providers

Mappers

Controllers

Mock APIs

Offline Queue

Retry Logic

---

# 29. Acceptance Criteria

- Feature-based architecture
- Riverpod only
- Repository pattern
- Immutable state
- Offline support
- Optimistic updates
- Streaming support
- Independent feature modules
- High testability
- Backend-aligned implementation

---

# Related Documents

- 00-Mobile-App-Vision.md
- 01-Design-System.md
- 02-Information-Architecture.md
- 03-Navigation-Architecture.md
- 04-API-Integration-Guide.md

---

**End of Document**
